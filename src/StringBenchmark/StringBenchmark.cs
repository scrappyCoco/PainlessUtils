using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using Coding4fun.PainlessUtils;

namespace StringBenchmark
{
    [MemoryDiagnoser]
    public class StringBenchmark
    {
        private const string Separator = "_";
        
        [Params("SampleTextForSimpleTest", "РусскийТекстПростоДляТеста")]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public string SampleText { get; set; } = null!;

        private TextRange[] _textRanges = null!;

        [GlobalSetup]
        public void Init()
        {
            _textRanges = SampleText.SplitWordRanges();
        }

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