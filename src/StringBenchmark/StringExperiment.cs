using System;
using System.Buffers;
using System.Text;
using Coding4fun.PainlessUtils;
using Range = Coding4fun.PainlessUtils.Range;

namespace StringBenchmark
{
    public static class StringExperiment
    {
        public static string SubStringAndStringBuilder(Range[] wordRanges, string sourceText,
            CaseRules.CharTransformationRule changeCaseRule, string? separator)
        {
            int separatorLength = separator?.Length ?? 0;
            int bufferSize = 0;
            foreach (Range textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            StringBuilder stringBuilder = new(bufferSize);

            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                Range wordRange = wordRanges[wordNumber];
                string word = sourceText.Substring(wordRange.Offset, wordRange.Length);
                
                for (int charNumber = 0; charNumber < word.Length; charNumber++)
                {
                    char ch = word[charNumber];
                    CharKind targetCharKind = changeCaseRule.Invoke(wordNumber, charNumber);
                    stringBuilder.Append(
                        targetCharKind == CharKind.Upper ? char.ToUpperInvariant(ch) :
                        targetCharKind == CharKind.Lower ? char.ToLowerInvariant(ch) :
                        ch);
                }

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    stringBuilder.Append(separator);
                }
            }

            return stringBuilder.ToString();
        }

        public static string SpanAndStringBuilder(Range[] wordRanges, string sourceText,
            CaseRules.CharTransformationRule changeCaseRule, string? separator)
        {
            int separatorLength = separator?.Length ?? 0;
            int bufferSize = 0;
            foreach (Range textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            StringBuilder stringBuilder = new(bufferSize);

            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                Range wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> wordSpan = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                for (int charNumber = 0; charNumber < wordSpan.Length; charNumber++)
                {
                    char ch = wordSpan[charNumber];
                    CharKind targetCharKind = changeCaseRule.Invoke(wordNumber, charNumber);
                    stringBuilder.Append(
                        targetCharKind == CharKind.Upper ? char.ToUpperInvariant(ch) :
                        targetCharKind == CharKind.Lower ? char.ToLowerInvariant(ch) :
                        ch);
                }

                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    stringBuilder.Append(separator);
                }
            }

            return stringBuilder.ToString();
        }

        public static string SpanAndCharArray(Range[] wordRanges, string sourceText, CaseRules.CharTransformationRule changeCaseRule,
            string? separator)
        {
            int separatorLength = separator?.Length ?? 0;
            int bufferSize = 0;
            foreach (Range textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            char[] textBuffer = new char[bufferSize];
            int offset = 0;
            
            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                Range wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> wordSpan = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                for (int charNumber = 0; charNumber < wordSpan.Length; charNumber++)
                {
                    char ch = wordSpan[charNumber];
                    CharKind targetCharKind = changeCaseRule.Invoke(wordNumber, charNumber);
                    textBuffer[offset++] = targetCharKind == CharKind.Upper ? char.ToUpperInvariant(ch) :
                        targetCharKind == CharKind.Lower ? char.ToLowerInvariant(ch) :
                        ch;
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
        public static string ArrayPool(Range[] wordRanges, string sourceText, CaseRules.CharTransformationRule changeCaseRule,
            string? separator)
        {
            int bufferSize = 0;
            int separatorLength = separator?.Length ?? 0;
            foreach (Range textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            char[] textBuffer = Pool.Rent(bufferSize);

            int offset = 0;
            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                Range wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);

                for (int charNumber = 0; charNumber < textRange.Length; charNumber++)
                {
                    char ch = textRange[charNumber];
                    CharKind targetCharKind = changeCaseRule.Invoke(wordNumber, charNumber);
                    textBuffer[offset++] = targetCharKind == CharKind.Upper ? char.ToUpperInvariant(ch) :
                        targetCharKind == CharKind.Lower ? char.ToLowerInvariant(ch) :
                        ch;
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
        
        public static string StackAlloc(Range[] wordRanges, string sourceText, CaseRules.CharTransformationRule changeCaseRule,
            string? separator)
        {
            int bufferSize = 0;
            int separatorLength = separator?.Length ?? 0;
            foreach (Range textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            Span<char> textBuffer = stackalloc char[bufferSize];
            int offset = 0;
            
            for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
            {
                Range wordRange = wordRanges[wordNumber];
                ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);
        
                for (int charNumber = 0; charNumber < textRange.Length; charNumber++)
                {
                    char ch = textRange[charNumber];
                    CharKind targetCharKind = changeCaseRule.Invoke(wordNumber, charNumber);
                    textBuffer[offset++] = targetCharKind == CharKind.Upper ? char.ToUpperInvariant(ch) :
                        targetCharKind == CharKind.Lower ? char.ToLowerInvariant(ch) :
                        ch;
                }
        
                if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                {
                    separator!.AsSpan().CopyTo(textBuffer.Slice(offset, separatorLength));
                    offset += separatorLength;
                }
            }
        
            string text;
            unsafe
            {
                fixed (char* textBufferPointer = textBuffer)
                {
                    text = new string(textBufferPointer, 0, textBuffer.Length);
                }
            }
        
            return text;
        }
        
        public static string UnsafeStringModification(Range[] wordRanges, string sourceText, CaseRules.CharTransformationRule changeCaseRule,
            string? separator)
        {
            int bufferSize = 0;
            int separatorLength = separator?.Length ?? 0;
            foreach (Range textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            int offset = 0;
            string text = new string(char.MinValue, bufferSize);
        
            unsafe
            {
                fixed (char* textPointer = text)
                {
                    for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
                    {
                        Range wordRange = wordRanges[wordNumber];
                        ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);
        
                        for (int charNumber = 0; charNumber < textRange.Length; charNumber++)
                        {
                            char ch = textRange[charNumber];
                            CharKind charKind = changeCaseRule.Invoke(wordNumber, charNumber);
                            textPointer[offset++] = charKind switch
                            {
                                CharKind.Lower => char.ToLowerInvariant(ch),
                                CharKind.Upper => char.ToUpperInvariant(ch),
                                _              => ch
                            };
                        }
        
                        if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                        {
                            foreach (char ch in separator!)
                            {
                                textPointer[offset++] = ch;
                            }
                        }
                    }
                }
            }
        
            return text;
        }
        
        public static string UnsafeStringModificationMask(Range[] wordRanges, string sourceText,
            CaseRules.CharTransformationRule changeCaseRule,
            string? separator, int[] lowerBits, int[] upperBits)
        {
            int bufferSize = 0;
            int separatorLength = separator?.Length ?? 0;
            foreach (Range textRange in wordRanges) bufferSize += textRange.Length;
            bufferSize += separatorLength * (wordRanges.Length - 1);
            int offset = 0;
            string text = new string(char.MinValue, bufferSize);
        
            unsafe
            {
                fixed (char* textPointer = text)
                {
                    int lowerMask = lowerBits[0];
                    int upperMask = upperBits[0];
                    int maskCharNumber = -1;
                    int maskIntNumber = 0;
                    
                    for (int wordNumber = 0; wordNumber < wordRanges.Length; wordNumber++)
                    {
                        Range wordRange = wordRanges[wordNumber];
                        ReadOnlySpan<char> textRange = sourceText.AsSpan(wordRange.Offset, wordRange.Length);
        
                        for (int charNumber = 0; charNumber < textRange.Length; charNumber++)
                        {
                            if (++maskCharNumber == 32)
                            {
                                maskCharNumber = 0;
                                ++maskIntNumber;
                                lowerMask = lowerBits[maskIntNumber];
                                upperMask = upperBits[maskIntNumber];
                            }

                            char ch = textRange[charNumber];
                            CharKind charKind = changeCaseRule.Invoke(wordNumber, charNumber);
                            // textPointer[offset++] = charKind switch
                            // {
                            //     CharKind.Lower => ((lowerMask >> maskCharNumber) & 1) == 1 ? ch : char.ToLowerInvariant(ch),
                            //     CharKind.Upper => ((upperMask >> maskCharNumber) & 1) == 1 ? ch : char.ToUpperInvariant(ch),
                            //     _              => ch
                            // };
                            if (charKind == CharKind.Lower)
                            {
                                if ((lowerMask & (1 << maskCharNumber)) == 0)
                                {
                                    textPointer[offset++] = char.ToLowerInvariant(ch);
                                }
                                else
                                {
                                    textPointer[offset++] = ch;
                                }
                            }
                            else if (charKind == CharKind.Upper)
                            {
                                if ((upperMask & (1 << maskCharNumber)) == 0)
                                {
                                    textPointer[offset++] = char.ToUpperInvariant(ch);
                                }
                                else
                                {
                                    textPointer[offset++] = ch;
                                }
                            }
                            else
                            {
                                textPointer[offset++] = ch;
                            }
                        }

                        if (separatorLength > 0 && wordNumber + 1 != wordRanges.Length)
                        {
                            foreach (char ch in separator!)
                            {
                                textPointer[offset++] = ch;
                            }
                        }
                    }
                }
            }
        
            return text;
        }
    }
}