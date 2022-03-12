using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using Coding4fun.PainlessUtils;

namespace StringBenchmark;

[MemoryDiagnoser]
public class CharKindBenchmark
{
    [Params("SampleTextForSimpleTest", "РусскийТекстПростоДляТеста")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public string SampleText { get; set; } = null!;
    
    [Benchmark(Baseline = true)]
    public CharKind[] GetCharKind()
    {
        CharKind[] charKinds = new CharKind[SampleText.Length];
        int pos = 0;
        foreach (char ch in SampleText)
        {
            CharKind charKind = GetCharKind(ch);
            charKinds[pos++] = charKind;
        }
        return charKinds;
    }
    
    [Benchmark]
    public CharKind[] GetUnicodeCategory()
    {
        CharKind[] charKinds = new CharKind[SampleText.Length];
        int pos = 0;
        foreach (char ch in SampleText)
        {
            CharKind charKind = GetUnicodeCategory(ch);
            charKinds[pos++] = charKind;
        }
        return charKinds;
    }

    private static CharKind GetCharKind(char ch) => char.IsDigit(ch) ? CharKind.Digit
        : char.IsUpper(ch) ? CharKind.Upper
        : char.IsLower(ch) ? CharKind.Lower
        : CharKind.Other;

    private static CharKind GetUnicodeCategory(char ch)
    {
        UnicodeCategory category = char.GetUnicodeCategory(ch);
        return category switch
        {
            UnicodeCategory.DecimalDigitNumber => CharKind.Digit,
            UnicodeCategory.UppercaseLetter    => CharKind.Upper,
            UnicodeCategory.LowercaseLetter    => CharKind.Lower,
            _                                  => CharKind.Other
        };
    }
}