namespace WagerService.Tests
{
    using System.Text.RegularExpressions;

    using FluentAssertions;
    
    using WagerService.Core.NumberGenerators;

    [TestFixture]
    public class RandomNumberGenerationTests
    {
        private RandomNumberGeneration generator;

        [SetUp]
        public void Setup()
        {
            generator = new RandomNumberGeneration();
        }

        [Test]
        public void Generate_ShouldReturn_NonNullOrEmptyString()
        {
            var result = generator.Generate();

            result.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Generate_ShouldReturn_NumericString()
        {
            var result = generator.Generate();

            Regex.IsMatch(result, @"^\d+$").Should().BeTrue();
        }

        [Test]
        public void Generate_ShouldReturn_FixedLengthStringOf19Digits()
        {
            var result = generator.Generate();

            result.Length.Should().Be(19);
        }

        [Test]
        public void Generate_ShouldProduceDifferentValues_MostOfTheTime()
        {
            var results = Enumerable.Range(0, 1000)
                                    .Select(_ => generator.Generate())
                                    .ToList();

            results.Distinct().Count().Should().BeGreaterThan(990);
        }
    }
}
