using BenchmarkDotNet.Running;


namespace StringBenchmark
{
    public static class Program
    {
        public static void Main()
        {
            //var summary = BenchmarkRunner.Run<StringBuilderVsBuffer>();
            var summary = BenchmarkRunner.Run<ResultBenchmark>();
            //new ResultBenchmark().SpanAndUnsafeAndByteArray();
        }
    }
}