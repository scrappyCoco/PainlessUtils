using System.Collections.Generic;
using System.Text.RegularExpressions;
using Coding4fun.PainlessUtils;
using NUnit.Framework;

namespace Coding4fun.PainlessString.Test
{
    public class Tests
    {
        private static readonly Regex TextRangeRegex = new(@"\[(?<word>.+?)\]");

        // @formatter:off
        private static IEnumerable<TestCaseData> GetSplitWordsData()
        {
            yield return new TestCaseData("CapitalizedString", "[Capitalized][String]");
            yield return new TestCaseData("UPPER_CASE",        "[UPPER]_[CASE]");
            yield return new TestCaseData("lowerCase",         "[lower][Case]");
            yield return new TestCaseData("kebab-case",        "[kebab]-[case]");
            yield return new TestCaseData("MIXText",           "[MIX][Text]");
            yield return new TestCaseData("MIX2Text",          "[MIX][2][Text]");
            yield return new TestCaseData("MIX_2_Text",        "[MIX]_[2]_[Text]");
            yield return new TestCaseData("data2text",         "[data][2][text]");
            yield return new TestCaseData("_bound_",           "_[bound]_");
            yield return new TestCaseData("_MIX_This_case_",   "_[MIX]_[This]_[case]_");
        }
        // @formatter:on

        [Test, TestCaseSource(nameof(GetSplitWordsData))]
        public void SplitWordRanges(string sourceText, string expectedRangeText)
        {
            TextRange[] expectedTextRanges = ParseTextRanges(expectedRangeText);
            TextRange[] actualTextRanges = sourceText.SplitWordRanges();
            Assert.AreEqual(expectedTextRanges, actualTextRanges);
        }

        [Test]
        public void ParseTextRanges()
        {
            const string rangedText = "[some][range]";
            TextRange[] textRanges = ParseTextRanges(rangedText);
            Assert.AreEqual(new TextRange(0, 4), textRanges[0]);
            Assert.AreEqual(new TextRange(4, 5), textRanges[1]);
            Assert.AreEqual(2, textRanges.Length);
        }

        private static TextRange[] ParseTextRanges(string sourceText)
        {
            var matches = TextRangeRegex.Matches(sourceText);
            TextRange[] textRanges = new TextRange[matches.Count];
            for (int matchNumber = 0; matchNumber < matches.Count; matchNumber++)
            {
                Match match = matches[matchNumber];
                Group matchGroup = match.Groups["word"];
                textRanges[matchNumber] = new TextRange(matchGroup.Index - (matchNumber * 2 + 1), matchGroup.Length);
            }

            return textRanges;
        }

        [Test]
        public void ChangeCase()
        {
            const string sourceText = "Source_Text";
            string? upperCase = sourceText.ChangeCase(CaseRules.ToUpperCase, "_");
            string? kebabCase = sourceText.ChangeCase(CaseRules.ToLowerCase, "-");
            string? camelCase = sourceText.ChangeCase(CaseRules.ToCamelCase, "");
            string? capitalized = sourceText.ChangeCase(CaseRules.ToCapitalizedCase, " ");
            string? titleCase = sourceText.ChangeCase(CaseRules.ToTitleCase, " ");

            Assert.AreEqual("SOURCE_TEXT", upperCase);
            Assert.AreEqual("source-text", kebabCase);
            Assert.AreEqual("sourceText", camelCase);
            Assert.AreEqual("Source text", capitalized);
            Assert.AreEqual("Source Text", titleCase);
        }
    }
}