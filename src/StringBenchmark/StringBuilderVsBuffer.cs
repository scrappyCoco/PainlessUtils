using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace StringBenchmark
{
    [MemoryDiagnoser]
    public class StringBuilderVsBuffer
    {
        private string _data;

        //[Params(10, 100, 1000)]
        public int StringLength { get; set; } = 1000;

        //[Params(2, 5, 10)]
        public int SubstringCount { get; set; } = 10;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _data = new string('a', StringLength);
        }

        [Benchmark]
        public string StringBuilder_Range_Substring()
        {
            StringBuilder sb = new();
            for (int i = 0; i < StringLength; i += SubstringCount)
            {
                sb.Append(_data.Substring(i, SubstringCount).ToUpperInvariant());
            }

            return sb.ToString();
        }

        [Benchmark]
        public string StringBuilder_Range_Slice()
        {
            StringBuilder stringBuilder = new();
            ReadOnlySpan<char> readOnlySpan = _data.AsSpan();

            for (int i = 0; i < StringLength; i += SubstringCount)
            {
                Span<char> upperSpan = new Span<char>(new char[SubstringCount]);
                ReadOnlySpan<char> onlySpan = readOnlySpan.Slice(i, SubstringCount);
                onlySpan.ToUpperInvariant(upperSpan);
                stringBuilder.Append(upperSpan);
            }

            return stringBuilder.ToString();
        }

        [Benchmark]
        public string Buffer_Range_Span()
        {
            ReadOnlySpan<char> readOnlySpan = _data.AsSpan();

            char[] charBuffer = new char[StringLength];
            int offset = 0;
            for (int i = 0; i < StringLength; i += SubstringCount)
            {
                Span<char> upperSpan = new Span<char>(charBuffer, offset, SubstringCount);
                offset += SubstringCount;
                var onlySpan = readOnlySpan.Slice(i, SubstringCount);
                onlySpan.ToUpperInvariant(upperSpan);
            }

            return new string(charBuffer);
        }

        [Benchmark]
        public string Buffer_Char_Span()
        {
            ReadOnlySpan<char> readOnlySpan = _data.AsSpan();

            char[] charBuffer = new char[StringLength];
            for (int i = 0; i < StringLength; i += SubstringCount)
            {
                var onlySpan = readOnlySpan.Slice(i, SubstringCount);
                foreach (char c in onlySpan)
                {
                    charBuffer[i++] = char.ToUpperInvariant(c);
                }
            }

            return new string(charBuffer);
        }

        [Benchmark]
        public string StringBuilder_Char_Slice()
        {
            StringBuilder stringBuilder = new();
            ReadOnlySpan<char> readOnlySpan = _data.AsSpan();
            for (int i = 0; i < StringLength; i += SubstringCount)
            {
                foreach (char c in readOnlySpan.Slice(i, SubstringCount))
                {
                    stringBuilder.Append(char.ToUpperInvariant(c));
                }
            }

            return stringBuilder.ToString();
        }
        
        [Benchmark]
        public string StringBuilderFixedLength_Char_Slice()
        {
            StringBuilder stringBuilder = new(StringLength);
            ReadOnlySpan<char> readOnlySpan = _data.AsSpan();
            for (int i = 0; i < StringLength; i += SubstringCount)
            {
                foreach (char c in readOnlySpan.Slice(i, SubstringCount))
                {
                    stringBuilder.Append(char.ToUpperInvariant(c));
                }
            }

            return stringBuilder.ToString();
        }
    }
}