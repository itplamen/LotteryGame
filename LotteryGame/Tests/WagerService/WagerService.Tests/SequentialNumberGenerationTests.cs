namespace WagerService.Tests
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using FluentAssertions;

    using WagerService.Core.NumberGenerators;

    [TestFixture]
    public class SequentialNumberGenerationTests
    {
        private SequentialNumberGeneration generator;

        [SetUp]
        public void Setup()
        {
            generator = new SequentialNumberGeneration();
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
        public void Generate_ShouldProduce_SequentialNumbers()
        {
            var first = generator.Generate();
            var second = generator.Generate();
            var third = generator.Generate();

            long.Parse(second).Should().Be(long.Parse(first) + 1);
            long.Parse(third).Should().Be(long.Parse(second) + 1);
        }

        [Test]
        public void Generate_ShouldProduceUniqueNumbers_WhenCalledMultipleTimes()
        {
            var results = Enumerable.Range(0, 1000)
                                    .Select(_ => generator.Generate())
                                    .ToList();

            results.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void Generate_ShouldBeThreadSafe()
        {
            var tasks = Enumerable.Range(0, 1000)
                                  .Select(_ => Task.Run(() => generator.Generate()))
                                  .ToArray();

            Task.WaitAll(tasks);

            var results = tasks.Select(t => t.Result).ToList();
            results.Should().OnlyHaveUniqueItems();
            results.Count.Should().Be(1000);
        }
    }
}
