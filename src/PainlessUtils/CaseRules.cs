using System;

namespace Coding4fun.PainlessUtils
{
    public static class CaseRules
    {
        public delegate char CharTransformationRule(int wordNumber, int charNumber, Char ch);

        public static char ToUpperCase(int wordNumber, int charNumber, Char ch) => char.ToUpperInvariant(ch);

        public static char ToLowerCase(int wordNumber, int charNumber, Char ch) => char.ToLowerInvariant(ch);

        public static char ToTitleCase(int wordNumber, int charNumber, Char ch) => charNumber == 0
            ? char.ToUpperInvariant(ch)
            : char.ToLowerInvariant(ch);

        public static char ToCapitalizedCase(int wordNumber, int charNumber, Char ch) =>
            wordNumber == 0 && charNumber == 0
                ? char.ToUpperInvariant(ch)
                : char.ToLowerInvariant(ch);

        public static char ToCamelCase(int wordNumber, int charNumber, Char ch) =>
            wordNumber > 0 && charNumber == 0
                ? char.ToUpperInvariant(ch)
                : char.ToLowerInvariant(ch);
    }
}