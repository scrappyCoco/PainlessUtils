using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coding4fun.PainlessString;

namespace StringBenchmark
{
    public static class SubstringStringExtension
    {
        
        /// <summary>
        /// Split text to words.
        /// </summary>
        /// <param name="text">Source text.</param>
        /// <returns>Words.</returns>
        public static string[] SplitWords(this string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<string>();
            }

            List<string> words = new();

            StringBuilder wordBuilder = new();

            CharKind currentCharKind = CharKind.Other;
            CharKind previousCharKind = CharKind.Other;
            for (int position = 0; position < text.Length; position++)
            {
                char currentChar = text[position];
                if (position != 0)
                {
                    previousCharKind = currentCharKind;
                }

                currentCharKind = GetCharKind(currentChar);

                if (currentCharKind == CharKind.Other)
                {
                    if (wordBuilder.Length > 0)
                    {
                        words.Add(wordBuilder.ToString());
                        wordBuilder.Clear();
                    }

                    continue;
                }

                wordBuilder.Append(currentChar);

                if (previousCharKind == CharKind.Other) continue;
                if (previousCharKind == currentCharKind) continue;
                if (previousCharKind == CharKind.Upper && currentCharKind == CharKind.Lower)
                {
                    // MIXName
                    //    ^^
                    // MIX must be added as word.
                    // Name will be next word.
                    if (wordBuilder.Length > 2)
                    {
                        string currentWord = wordBuilder.ToString();
                        words.Add(currentWord[..^2]);
                        wordBuilder.Remove(0, currentWord.Length - 2);
                    }

                    continue;
                }

                wordBuilder.Remove(wordBuilder.Length - 1, 1);
                words.Add(wordBuilder.ToString());
                wordBuilder.Clear();
                wordBuilder.Append(currentChar);
            }

            if (wordBuilder.Length > 0)
            {
                words.Add(wordBuilder.ToString());
            }

            return words.ToArray();
        }

        /// <summary>
        ///     Returns text in title case - all words will be capitalized.
        /// </summary>
        /// <example>
        /// CapitalizedString -> Capitalized String
        /// UPPER_CASE        -> Upper Case
        /// lowerCase         -> Lower Case
        /// kebab-case        -> Kebab Case
        /// MIXText           -> Mix Text
        /// data2text         -> Data 2 Text
        /// </example>
        /// <param name="text">Source text.</param>
        /// <param name="separator">Separator between words.</param>
        /// <returns>Text in title case.</returns>
        public static string? ToTitleCase(this string? text, string? separator = " ") =>
            ChangeCase(text, separator, (word, _) => FirstCharToUpperCase(word));

        /// <summary>
        ///     Returns text in upper case.
        /// </summary>
        /// <example>
        /// CapitalizedString -> Capitalized String
        /// UPPER_CASE        -> UPPER CASE
        /// lowerCase         -> LOWER CASE
        /// kebab-case        -> KEBAB CASE
        /// MIXText           -> MIX TEXT
        /// data2text         -> DATA 2 TEXT
        /// </example>
        /// <param name="text">Source text.</param>
        /// <param name="separator">Separator between words.</param>
        /// <returns>Text in upper case.</returns>
        public static string? ToUpperCase(this string? text, string? separator = " ") =>
            ChangeCase(text, separator, (word, _) => word.ToUpperInvariant());

        /// <summary>
        ///     Returns text in lower case.
        /// </summary>
        /// <example>
        /// CapitalizedString -> capitalized string
        /// UPPER_CASE        -> upper case
        /// lowerCase         -> lower case
        /// kebab-case        -> kebab case
        /// MIXText           -> mix text
        /// data2text         -> data 2 text
        /// </example>
        /// <param name="text">Source text.</param>
        /// <param name="separator">Separator between words.</param>
        /// <returns>Text in lower case.</returns>
        public static string? ToLowerCase(this string? text, string? separator = " ") =>
            ChangeCase(text, separator, (word, _) => word.ToLowerInvariant());

        /// <summary>
        ///     Returns text in capitalized case.
        /// </summary>
        /// <example>
        /// CapitalizedString -> Capitalized string
        /// UPPER_CASE        -> Upper case
        /// lowerCase         -> Lower case
        /// kebab-case        -> Kebab case
        /// MIXText           -> Mix text
        /// data2text         -> Data 2 text
        /// </example>
        /// <param name="text">Source text.</param>
        /// <param name="separator">Separator between words.</param>
        /// <returns>Text in capitalized case.</returns>
        public static string? Capitalize(this string? text, string? separator = " ") =>
            ChangeCase(text, separator, CapitalizeWord);

        private static string? ChangeCase(string? text, string? separator,
            Func<string, int, string> wordTransformationFunc)
        {
            string[] words = text.SplitWords();
            if (words.Length == 0)
            {
                return null;
            }

            string?[] transformedWords = words.Select(wordTransformationFunc).ToArray();
            return string.Join(separator, transformedWords);
        }

        private static string FirstCharToUpperCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            if (text.Length == 1) return text.ToUpperInvariant();
            return text[0].ToString().ToUpperInvariant() + text[1..].ToLowerInvariant();
        }

        private static string CapitalizeWord(string word, int wordNumber)
        {
            if (wordNumber == 0) return FirstCharToUpperCase(word);
            return word.ToLowerInvariant();
        }
        
        private static CharKind GetCharKind(char ch) => char.IsUpper(ch) ? CharKind.Upper
            : char.IsLower(ch) ? CharKind.Lower
            : char.IsDigit(ch) ? CharKind.Digit
            : CharKind.Other;
    }
}