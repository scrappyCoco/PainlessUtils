using BenchmarkDotNet.Running;


namespace StringBenchmark
{
    public static class Program
    {
        public static void Main()
        {
            var summary = BenchmarkRunner.Run<StringBenchmark>();
        }
    }
}