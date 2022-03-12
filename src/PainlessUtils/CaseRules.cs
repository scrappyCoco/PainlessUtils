using System;

namespace Coding4fun.PainlessUtils
{
    public static class CaseRules
    {
        public delegate CharKind CharTransformationRule(int wordNumber, int charNumber);

        public static CharKind ToUpperCase(int wordNumber, int charNumber) => CharKind.Upper;

        public static CharKind ToLowerCase(int wordNumber, int charNumber) => CharKind.Lower;

        public static CharKind ToTitleCase(int wordNumber, int charNumber) => charNumber == 0
            ? CharKind.Upper
            : CharKind.Lower;

        public static CharKind ToCapitalizedCase(int wordNumber, int charNumber) =>
            wordNumber == 0 && charNumber == 0
                ? CharKind.Upper
                : CharKind.Lower;

        public static CharKind ToCamelCase(int wordNumber, int charNumber) =>
            wordNumber > 0 && charNumber == 0
                ? CharKind.Upper
                : CharKind.Lower;
    }
}