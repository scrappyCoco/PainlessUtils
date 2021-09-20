using BenchmarkDotNet.Attributes;
using Coding4fun.PainlessString;

namespace StringBenchmark
{
    [MemoryDiagnoser]
    public class ResultBenchmark
    {
        public string SampleText { get; } = "SampleText";

        [Benchmark]
        public string Painless()
        {
            return SampleText.ChangeCase(CaseRules.ToUpperCase, "_");
        }

        [Benchmark]
        public string Substring()
        {
            return SampleText.ToUpperCase("_");
        }
    }
}