using System;
using System.Buffers;
using System.Text;
using Coding4fun.PainlessUtils;

namespace StringBenchmark
{
    public static class StringExperiment
    {
        public static string SubStringAndStringBuilder(TextRange[] wordRanges, string sourceText,
            CaseRules.CharTransformationRule changeCaseRule, string? separator)
        {
            int separatorLength = separator?.Length ?? 0;
            int bufferSize = 0;
            foreach (TextRange textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            StringBuilder stringBuilder = new(bufferSize);

            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                TextRange wordRange = wordRanges[wordNumber];
                string word = sourceText.Substring(wordRange.Offset, wordRange.Length);
                
                for (int charNumber = 0; charNumber < word.Length; charNumber++)
                {
                    char ch = word[charNumber];
                    ch = changeCaseRule.Invoke(wordNumber, charNumber, ch);
                    stringBuilder.Append(ch);
                }

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    stringBuilder.Append(separator);
                }
            }

            return stringBuilder.ToString();
        }

        public static string SpanAndStringBuilder(TextRange[] wordRanges, string sourceText,
            CaseRules.CharTransformationRule changeCaseRule, string? separator)
        {
            int separatorLength = separator?.Length ?? 0;
            int bufferSize = 0;
            foreach (TextRange textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            StringBuilder stringBuilder = new(bufferSize);

            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                TextRange wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> wordSpan = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                for (int charNumber = 0; charNumber < wordSpan.Length; charNumber++)
                {
                    char ch = wordSpan[charNumber];
                    ch = changeCaseRule.Invoke(wordNumber, charNumber, ch);
                    stringBuilder.Append(ch);
                }

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    stringBuilder.Append(separator);
                }
            }

            return stringBuilder.ToString();
        }

        public static string SpanAndCharArray(TextRange[] wordRanges, string sourceText, CaseRules.CharTransformationRule changeCaseRule,
            string? separator)
        {
            int separatorLength = separator?.Length ?? 0;
            int bufferSize = 0;
            foreach (TextRange textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            char[] textBuffer = new char[bufferSize];
            int offset = 0;
            
            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                TextRange wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> wordSpan = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                for (int charNumber = 0; charNumber < wordSpan.Length; charNumber++)
                {
                    char ch = wordSpan[charNumber];
                    ch = changeCaseRule.Invoke(wordNumber, charNumber, ch);
                    textBuffer[offset++] = ch;
                }

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    separator!.AsSpan().CopyTo(textBuffer.AsSpan(offset, separatorLength));
                    offset += separatorLength;
                }
            }

            return new string(textBuffer);
        }

        
        private static readonly ArrayPool<char> Pool = ArrayPool<char>.Create();
        public static string ArrayPool(TextRange[] wordRanges, string sourceText, CaseRules.CharTransformationRule changeCaseRule,
            string? separator)
        {
            int bufferSize = 0;
            int separatorLength = separator?.Length ?? 0;
            foreach (TextRange textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            char[] textBuffer = Pool.Rent(bufferSize);

            int offset = 0;
            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                TextRange wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                for (int charNumber = 0; charNumber < textRange.Length; charNumber++)
                {
                    char ch = textRange[charNumber];
                    ch = changeCaseRule.Invoke(wordNumber, charNumber, ch);
                    textBuffer[offset++] = ch;
                }

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    separator!.AsSpan().CopyTo(textBuffer.AsSpan(offset, separatorLength));
                    offset += separatorLength;
                }
            }

            string text = new string(textBuffer);
            Pool.Return(textBuffer);
            
            return text;
        }
        
        public static string StackAlloc(TextRange[] wordRanges, string sourceText, CaseRules.CharTransformationRule changeCaseRule,
            string? separator)
        {
            int bufferSize = 0;
            int separatorLength = separator?.Length ?? 0;
            foreach (TextRange textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            Span<char> textBuffer = stackalloc char[bufferSize];
            int offset = 0;
            
            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                TextRange wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                for (int charNumber = 0; charNumber < textRange.Length; charNumber++)
                {
                    char ch = textRange[charNumber];
                    ch = changeCaseRule.Invoke(wordNumber, charNumber, ch);
                    textBuffer[offset++] = ch;
                }

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    separator!.AsSpan().CopyTo(textBuffer.Slice(offset, separatorLength));
                    offset += separatorLength;
                }
            }

            string text = new(textBuffer);

            return text;
        }
    }
}