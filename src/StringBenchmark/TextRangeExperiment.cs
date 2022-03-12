using System;
using System.Collections.Generic;
using Coding4fun.PainlessUtils;
using Range = Coding4fun.PainlessUtils.Range;

namespace StringBenchmark;

public class TextRangeExperiment
{
    public static Range[] SplitWordRanges(string? sourceText)
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

            currentCharKind = StringExtensions.GetCharKind(currentChar);

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
    
    public static void SplitWordRangesWithCase(string? sourceText, out Range[] ranges, out int[] upperBits, out int[] lowerBits)
    {
        if (string.IsNullOrWhiteSpace(sourceText))
        {
            ranges = Array.Empty<Range>();
            upperBits = Array.Empty<int>();
            lowerBits = Array.Empty<int>();
            return;
        }

        List<Range> words = new List<Range>(sourceText.Length / 5 + 1);

        CharKind currentCharKind = CharKind.Other;
        CharKind previousCharKind = CharKind.Other;
        int wordOffset = 0;
        int wordLength = 0;
        int position = 0;

        int bytesCountForMask = sourceText.Length / 32 + (sourceText.Length % 32 == 0 ? 0 : 1);
        int currentUpperMask = 0;
        int currentLowerMask = 0;
        int charOffset = 0;
        int intOffset = 0;
        upperBits = new int[bytesCountForMask];
        lowerBits = new int[bytesCountForMask];

        void FlushWord()
        {
            if (wordLength == 0) return;
            words.Add(new Range(wordOffset, wordLength));
        }

        for (; position < sourceText.Length; ++position)
        {
            char currentChar = sourceText[position];
            if (position != 0) previousCharKind = currentCharKind;

            currentCharKind = StringExtensions.GetCharKind(currentChar);

            if (currentCharKind == CharKind.Other)
            {
                FlushWord();
                wordOffset = position + 1;
                wordLength = 0;
                continue;
            }

            if (currentCharKind == CharKind.Upper)
            {
                currentUpperMask |= 1 << charOffset;
            }
            else if (currentCharKind == CharKind.Lower)
            {
                currentLowerMask |= 1 << charOffset;
            }
            
            if (++charOffset == 32)
            {
                upperBits[intOffset] = currentUpperMask;
                lowerBits[intOffset] = currentLowerMask;
                ++intOffset;
                charOffset = 0;
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
            
            if (charOffset > 0)
            {
                upperBits[intOffset] = currentUpperMask;
                lowerBits[intOffset] = currentLowerMask;
            }
        }

        FlushWord();

        ranges = words.ToArray();
    }
}