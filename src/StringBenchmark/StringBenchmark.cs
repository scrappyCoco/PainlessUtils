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
        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
        public string SampleText
        {
            get => _sampleText;
            set
            {
                _sampleText = value;
                TextRangeExperiment.SplitWordRangesWithCase(SampleText, out _textRanges, out _upperMask, out _lowerMask);
            }
        }

        private Range[] _textRanges = null!;
        private int[] _lowerMask = null!;
        private int[] _upperMask = null!;
        private string _sampleText = null!;
        //
        // [Benchmark(Baseline = true)]
        // public string SubStringAndStringBuilder()
        // {
        //     return StringExperiment.SubStringAndStringBuilder(_textRanges, SampleText, CaseRules.ToUpperCase,
        //         Separator);
        // }
        //
        // [Benchmark]
        // public string SpanAndStringBuilder()
        // {
        //     return StringExperiment.SpanAndStringBuilder(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        // }
        //
        // [Benchmark]
        // public string SpanAndCharArray()
        // {
        //     return StringExperiment.SpanAndCharArray(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        // }
        //
        // [Benchmark]
        // public string ArrayPool()
        // {
        //     return StringExperiment.ArrayPool(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        // }
        //
        // [Benchmark]
        // public string StackAlloc() =>
        //     StringExperiment.StackAlloc(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        //
        // [Benchmark]
        // public string UnsafeStringModificationUpper() =>
        //     StringExperiment.UnsafeStringModification(_textRanges, SampleText, CaseRules.ToUpperCase, Separator);
        //
        // [Benchmark]
        // public string UnsafeStringModificationMaskUpper() =>
        //     StringExperiment.UnsafeStringModificationMask(_textRanges, SampleText, CaseRules.ToUpperCase, Separator, _lowerMask, _upperMask);
        //
        [Benchmark]
        public string UnsafeStringModificationLower() =>
            StringExperiment.UnsafeStringModification(_textRanges, SampleText, CaseRules.ToLowerCase, Separator);
        
        [Benchmark]
        public string UnsafeStringModificationMaskLower() =>
            StringExperiment.UnsafeStringModificationMask(_textRanges, SampleText, CaseRules.ToLowerCase, Separator, _lowerMask, _upperMask);
    }
}