using BenchmarkDotNet.Attributes;
using Coding4fun.PainlessUtils;

namespace StringBenchmark
{
    [MemoryDiagnoser]
    public class StringBenchmark
    {
        private const string SampleText = "SampleTextForSimpleTest";
        private const string Separator = "_";
        private readonly TextRange[] _textRanges = SampleText.SplitWordRanges();

        [Benchmark(Baseline = true)]
        public string SubStringAndStringBuilder()
        {
            return StringExperiment.SubStringAndStringBuilder(_textRanges, SampleText, CaseRules.ToUpperCase,
                Separator);
        }

        [Benchmark]
        public string SpanAndStringBuilder()
        {
            return StringExperiment.SpanAndStringBuilder(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        }

        [Benchmark]
        public string SpanAndCharArray()
        {
            return StringExperiment.SpanAndCharArray(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        }

        [Benchmark]
        public string ArrayPool()
        {
            return StringExperiment.ArrayPool(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        }

        [Benchmark]
        public string StackAlloc() =>
            StringExperiment.StackAlloc(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        
        [Benchmark]
        public string UnsafeStringModification() =>
            StringExperiment.UnsafeStringModification(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
    }
}