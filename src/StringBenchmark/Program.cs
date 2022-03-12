using BenchmarkDotNet.Running;
using Coding4fun.PainlessUtils;

namespace StringBenchmark
{
    public static class Program
    {
        public static void Main()
        {
            //var summary = BenchmarkRunner.Run<CharKindBenchmark>();
            var summary = BenchmarkRunner.Run<StringBenchmark>();

            // const string text = "РусскийТекстПростоДляТеста";
            // TextRangeExperiment.SplitWordRangesWithCase(text, out Range[] ranges, out int[] upperBits, out int[] lowerBits);
            // StringExperiment.UnsafeStringModificationMask(ranges, "РусскийТекстПростоДляТеста", CaseRules.ToLowerCase, "_", lowerBits, upperBits);
        }
    }
}