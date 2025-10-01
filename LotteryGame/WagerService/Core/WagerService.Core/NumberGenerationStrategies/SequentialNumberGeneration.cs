namespace WagerService.Core.NumberGenerators
{
    using WagerService.Core.Contracts;

    public class SequentialNumberGeneration : INumberGeneration
    {
        private long counter = 1;

        public string Generate() => Interlocked.Increment(ref counter).ToString("D19");
    }
}
