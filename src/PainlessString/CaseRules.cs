using System;

namespace Coding4fun.PainlessString
{
    public static class CaseRules
    {
        public delegate void Rule(ReadOnlySpan<char> word, int wordNumber, Action<char> consumer);

        public static void ToUpperCase(ReadOnlySpan<char> word, int wordNumber, Action<char> consumer)
        {
            foreach (char ch in word) consumer.Invoke(char.ToUpperInvariant(ch));
        }

        public static void ToLowerCase(ReadOnlySpan<char> word, int wordNumber, Action<char> consumer)
        {
            foreach (char ch in word) consumer.Invoke(char.ToLowerInvariant(ch));
        }

        public static void ToTitleCase(ReadOnlySpan<char> word, int wordNumber, Action<char> consumer)
        {
            for (int chNumber = 0; chNumber < word.Length; chNumber++)
            {
                char ch = word[chNumber];
                consumer.Invoke(chNumber == 0
                    ? char.ToUpperInvariant(ch)
                    : char.ToLowerInvariant(ch));
            }
        }

        public static void ToCapitalizedCase(ReadOnlySpan<char> word, int wordNumber, Action<char> consumer)
        {
            for (int chNumber = 0; chNumber < word.Length; chNumber++)
            {
                char ch = word[chNumber];
                consumer.Invoke(wordNumber == 0 && chNumber == 0
                    ? char.ToUpperInvariant(ch)
                    : char.ToLowerInvariant(ch));
            }
        }

        public static void ToCamelCase(ReadOnlySpan<char> word, int wordNumber, Action<char> consumer)
        {
            for (int chNumber = 0; chNumber < word.Length; chNumber++)
            {
                char ch = word[chNumber];
                consumer.Invoke(wordNumber > 0 && chNumber == 0
                    ? char.ToUpperInvariant(ch)
                    : char.ToLowerInvariant(ch));
            }
        }
    }
}