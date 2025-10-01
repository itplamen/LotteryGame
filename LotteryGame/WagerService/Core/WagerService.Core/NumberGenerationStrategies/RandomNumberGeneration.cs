namespace WagerService.Core.NumberGenerators
{
    using WagerService.Core.Contracts;

    public class RandomNumberGeneration : INumberGeneration
    {
        public string Generate() => Random.Shared.Next(1, int.MaxValue).ToString("D19");
    }
}
