using System;
using System.Text;
using Coding4fun.PainlessUtils;

namespace StringBenchmark
{
    public static class StringExperiment
    {
        // Substring + SB
        // Span + SB
        // Span + byte[]

        public static string SubStringAndStringBuilder(TextRange[] wordRanges, string sourceText,
            CaseRules.Rule changeCaseRule, string? separator)
        {
            int bufferSize = 0;
            foreach (TextRange textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += (separator?.Length ?? 0) * (wordRanges.Length - 1);

            StringBuilder stringBuilder = new(bufferSize);

            int separatorLength = separator?.Length ?? 0;

            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                TextRange wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> textRange = sourceText.Substring(wordRange.Offset, wordRange.Length).AsSpan();

                void CharConsumer(char ch) => stringBuilder.Append(ch);
                changeCaseRule.Invoke(textRange, wordNumber, CharConsumer);

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    stringBuilder.Append(separator);
                }
            }

            return stringBuilder.ToString();
        }

        public static string SpanAndStringBuilder(TextRange[] wordRanges, string sourceText,
            CaseRules.Rule changeCaseRule, string? separator)
        {
            int bufferSize = 0;
            foreach (TextRange textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += (separator?.Length ?? 0) * (wordRanges.Length - 1);

            StringBuilder stringBuilder = new(bufferSize);

            int separatorLength = separator?.Length ?? 0;

            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                TextRange wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                void CharConsumer(char ch) => stringBuilder.Append(ch);
                changeCaseRule.Invoke(textRange, wordNumber, CharConsumer);

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    stringBuilder.Append(separator);
                }
            }

            return stringBuilder.ToString();
        }

        public static string SpanAndCharArray(TextRange[] wordRanges, string sourceText, CaseRules.Rule changeCaseRule,
            string? separator)
        {
            int bufferSize = 0;
            foreach (TextRange textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += (separator?.Length ?? 0) * (wordRanges.Length - 1);
            char[] textBuffer = new char[bufferSize];

            int separatorLength = separator?.Length ?? 0;

            int charNumber = 0;
            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                TextRange wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                void CharConsumer(char ch) => textBuffer[charNumber++] = ch;
                changeCaseRule.Invoke(textRange, wordNumber, CharConsumer);

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    separator!.AsSpan().CopyTo(textBuffer.AsSpan(charNumber, separatorLength));
                    charNumber += separatorLength;
                }
            }

            return new string(textBuffer);
        }
    }
}