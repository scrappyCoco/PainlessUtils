using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Range = Coding4fun.PainlessUtils.Range;

namespace Coding4fun.PainlessString.Test
{
    public class RangeTests
    {
        [Test]
        public void ContainsValue()
        {
            Range range = new(1, 10);
            Assert.True(range.Contains(1));
            Assert.True(range.Contains(5));
            Assert.True(range.Contains(10));
            Assert.False(range.Contains(0));
            Assert.False(range.Contains(11));
        }

        public struct RangeData
        {
            public readonly Range SourceRange;
            public readonly Range SecondRange;
            public readonly Range? ExpectedRange;

            public RangeData(string sourceRange, string secondRange, string? expectedRange = null)
            {
                SourceRange = Parse(sourceRange)!.Value;
                SecondRange = Parse(secondRange)!.Value;
                ExpectedRange = Parse(expectedRange);
            }
            
            private static readonly Regex PlusRangeRegex = new Regex("(\\++)");
            private static Range? Parse(string? text)
            {
                if (text == null) return null;
                Match match = PlusRangeRegex.Match(text);
                Group group = match.Groups[1];
                return new(group.Index, group.Length);
            }
        }

        [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
        private static IEnumerable<RangeData> ValidSubtractCases
        {
            get
            {
                // @formatter:off
                yield return new RangeData(sourceRange:   "  ++++++++  ",
                                           secondRange:   "     +++++++",
                                           expectedRange: "  +++       ");
                yield return new RangeData(sourceRange:   "  ++++++++  ",
                                           secondRange:   "   +++++++  ",
                                           expectedRange: "  +         ");
                yield return new RangeData(sourceRange:   "  ++++++++  ",
                                           secondRange:   "+++++++     ",
                                           expectedRange: "       +++  ");
                yield return new RangeData(sourceRange:   "  ++++++++  ",
                                           secondRange:   "  +++++++   ",
                                           expectedRange: "         +  ");
                // @formatter:on
            }
        }

        [TestCaseSource(nameof(ValidSubtractCases))]
        public void ValidSubtract(RangeData data)
        {
            Range resultRange = data.SourceRange - data.SecondRange;
            Assert.AreEqual(data.ExpectedRange, resultRange);
        }
        
        [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
        private static IEnumerable<RangeData> InvalidSubtractCases
        {
            get
            {
                // @formatter:off
                yield return new RangeData(sourceRange: "  ++++++++  ",
                                           secondRange: "  ++++++++  ");
                yield return new RangeData(sourceRange: "  ++++++++  ",
                                           secondRange: "   ++++++   ");
                yield return new RangeData(sourceRange: "  ++++++++  ",
                                           secondRange: " ++++++++++ ");
                yield return new RangeData(sourceRange: "  ++++++++  ",
                                           secondRange: " +++++++++  ");
                yield return new RangeData(sourceRange: "  ++++++++  ",
                                           secondRange: "  +++++++++ ");
                yield return new RangeData(sourceRange: "  ++        ",
                                           secondRange: "    ++      ");
                // @formatter:on
            }
        }

        [TestCaseSource(nameof(InvalidSubtractCases))]
        public void InvalidSubtract(RangeData data)
        {
            Assert.Catch<InvalidOperationException>(() =>
            {
                var _ = data.SourceRange - data.SecondRange;
            }, "Source range: {0}, Second range: {1}, Expected range: {2}", data.SourceRange, data.SecondRange, data.ExpectedRange);
        }
        
        [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
        private static IEnumerable<RangeData> InvalidAddCases
        {
            get
            {
                // @formatter:off
                yield return new RangeData(sourceRange: "  ++        ",
                                           secondRange: "     ++     ");
                yield return new RangeData(sourceRange: "     ++     ",
                                           secondRange: "  ++        ");
                // @formatter:on
            }
        }

        [TestCaseSource(nameof(InvalidAddCases))]
        public void InvalidAdd(RangeData data)
        {
            Assert.Catch<InvalidOperationException>(() =>
            {
                var _ = data.SourceRange + data.SecondRange;
            });
        }
        
        [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
        private static IEnumerable<RangeData> ValidAddCases
        {
            get
            {
                // @formatter:off
                yield return new RangeData(sourceRange:   "  ++   ",
                                           secondRange:   "    ++ ",
                                           expectedRange: "  ++++ ");
                yield return new RangeData(sourceRange:   "    ++ ",
                                           secondRange:   "  ++   ",
                                           expectedRange: "  ++++ ");
                yield return new RangeData(sourceRange:   "    +++",
                                           secondRange:   "  +++  ",
                                           expectedRange: "  +++++");
                // @formatter:on
            }
        }
        
        [TestCaseSource(nameof(ValidAddCases))]
        public void ValidAdd(RangeData data)
        {
            Range resultRange = data.SourceRange + data.SecondRange;
            Assert.AreEqual(data.ExpectedRange, resultRange);
        }
        
        [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
        private static IEnumerable<RangeData> IntersectCases
        {
            get
            {
                // @formatter:off
                yield return new RangeData(sourceRange:   " ++  ",
                                           secondRange:   "  ++ ");
                yield return new RangeData(sourceRange:   "  ++ ",
                                           secondRange:   " ++  ");
                yield return new RangeData(sourceRange:   "  +++ ",
                                           secondRange:   "  +++ ");
                yield return new RangeData(sourceRange:   "  + ",
                                           secondRange:   "  + ");
                // @formatter:on
            }
        }
        
        [TestCaseSource(nameof(IntersectCases))]
        public void Intersect(RangeData data)
        {
            bool intersect = data.SourceRange.Intersect(data.SecondRange);
            Assert.IsTrue(intersect);
        }
        
        [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
        private static IEnumerable<RangeData> NotIntersectCases
        {
            get
            {
                // @formatter:off
                yield return new RangeData(sourceRange:   " ++   ",
                                           secondRange:   "   ++ ",
                                           expectedRange: null);
                yield return new RangeData(sourceRange:   "   ++ ",
                                           secondRange:   " ++   ",
                                           expectedRange: null);
                yield return new RangeData(sourceRange:   " +  ",
                                           secondRange:   "  + ",
                                           expectedRange: null);
                // @formatter:on
            }
        }
        
        [TestCaseSource(nameof(NotIntersectCases))]
        public void NotIntersect(RangeData data)
        {
            bool intersect = data.SourceRange.Intersect(data.SecondRange);
            Assert.IsFalse(intersect);
        }
        
        [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
        private static IEnumerable<RangeData> ContainsCases
        {
            get
            {
                // @formatter:off
                yield return new RangeData(sourceRange:   " +++ ",
                                           secondRange:   "  ++ ");
                yield return new RangeData(sourceRange:   " ++  ",
                                           secondRange:   " ++  ");
                yield return new RangeData(sourceRange:   " +++ ",
                                           secondRange:   " ++  ");
                yield return new RangeData(sourceRange:   " +++ ",
                                           secondRange:   "  +  ");
                // @formatter:on
            }
        }
        
        [TestCaseSource(nameof(ContainsCases))]
        public void ContainsRange(RangeData data)
        {
            bool contains = data.SourceRange.Contains(data.SecondRange);
            Assert.IsTrue(contains);
        }
        
        [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
        [SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
        private static IEnumerable<RangeData> NotContainsCases
        {
            get
            {
                // @formatter:off
                yield return new RangeData(sourceRange: " +++  ",
                                           secondRange: "   ++ ");
                yield return new RangeData(sourceRange: "  ++  ",
                                           secondRange: " ++   ");
                yield return new RangeData(sourceRange: " +++  ",
                                           secondRange: "    ++");
                yield return new RangeData(sourceRange: " +++ ",
                                           secondRange: "+    ");
                // @formatter:on
            }
        }
        
        [TestCaseSource(nameof(NotContainsCases))]
        public void NotContainsRange(RangeData data)
        {
            bool contains = data.SourceRange.Contains(data.SecondRange);
            Assert.IsFalse(contains);
        }
    }
}