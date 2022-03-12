using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;

namespace Coding4fun.PainlessUtils
{
    public static class StringExtensions
    {
        private const int MaxSizeInStack = 1024;

        public static Range[] SplitWordRanges(this string? sourceText)
        {
            if (string.IsNullOrWhiteSpace(sourceText)) return Array.Empty<Range>();

            List<Range> words = new List<Range>(sourceText!.Length / 5 + 1);

            CharKind currentCharKind = CharKind.Other;
            CharKind previousCharKind = CharKind.Other;
            int wordOffset = 0;
            int wordLength = 0;
            int position = 0;

            void FlushWord()
            {
                if (wordLength == 0) return;
                words.Add(new Range(wordOffset, wordLength));
            }

            for (; position < sourceText.Length; ++position)
            {
                char currentChar = sourceText[position];
                if (position != 0) previousCharKind = currentCharKind;

                currentCharKind = GetCharKind(currentChar);

                if (currentCharKind == CharKind.Other)
                {
                    FlushWord();
                    wordOffset = position + 1;
                    wordLength = 0;
                    continue;
                }

                ++wordLength;

                if (wordLength == 1) continue;
                if (previousCharKind == CharKind.Other) continue;
                if (previousCharKind == currentCharKind) continue;
                if (previousCharKind == CharKind.Upper && currentCharKind == CharKind.Lower)
                {
                    // MIXName
                    //    ^^
                    // MIX must be added as word.
                    // Name will be next word.
                    if (wordLength > 2)
                    {
                        wordLength -= 2;
                        FlushWord();
                        wordOffset = position - 1;
                        wordLength = 2;
                    }

                    continue;
                }

                --wordLength;
                FlushWord();
                wordOffset = position;
                wordLength = 1;
            }

            FlushWord();

            return words.ToArray();
        }

        public static string? ChangeCase(this string? sourceText, CaseRules.CharTransformationRule changeCaseRule,
            string? separator = null)
        {
            Range[] wordRanges = sourceText.SplitWordRanges();
            if (wordRanges.Length == 0) return null;

            int separatorLength = separator?.Length ?? 0;
            int bufferSize = 0;
            int offset = 0;
            foreach (Range textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);

            bool isLongText = bufferSize > MaxSizeInStack;
            char[]? arrayFromHeap = null;
            if (isLongText)
            {
                arrayFromHeap = ArrayPool<char>.Shared.Rent(bufferSize);
            }

            Span<char> textBuffer = isLongText ? arrayFromHeap.AsSpan() : stackalloc char[bufferSize];

            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                Range wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                for (int charNumber = 0; charNumber < textRange.Length; charNumber++)
                {
                    char ch = textRange[charNumber];
                    //ch = changeCaseRule.Invoke(wordNumber, charNumber, ch);
                    textBuffer[offset++] = ch;
                }

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    separator!.ToCharArray().CopyTo(textBuffer.Slice(offset, separatorLength));
                    offset += separatorLength;
                }
            }

            string resultText;
            unsafe
            {
                fixed (char* textBufferPointer = textBuffer)
                {
                    resultText = new string(textBufferPointer, 0, textBuffer.Length);
                }
            }

            if (isLongText)
            {
                ArrayPool<char>.Shared.Return(arrayFromHeap);
            }

            return resultText;
        }

        internal static CharKind GetCharKind(char ch) =>
            char.GetUnicodeCategory(ch) switch
            {
                UnicodeCategory.DecimalDigitNumber => CharKind.Digit,
                UnicodeCategory.UppercaseLetter    => CharKind.Upper,
                UnicodeCategory.LowercaseLetter    => CharKind.Lower,
                _                                  => CharKind.Other
            };
    }
}