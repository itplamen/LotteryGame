namespace WagerService.Tests
{
    using FluentAssertions;
    
    using Microsoft.Extensions.Configuration;

    using WagerService.Core.NumberGenerators;

    [TestFixture]
    public class SnowflakeNumberGeneratorTests
    {
        private IConfiguration config;
        private SnowflakeNumberGenerator generator;

        [SetUp]
        public void Setup()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Snowflake:Epoch", "2020-01-01T00:00:00Z"},
                {"Snowflake:NodeId", "1"}
            };

            config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            generator = new SnowflakeNumberGenerator(config);
        }

        [Test]
        public void Constructor_ShouldThrow_WhenNodeIdOutOfRange()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Snowflake:Epoch", "2020-01-01T00:00:00Z"},
                {"Snowflake:NodeId", "1024"}
            };

            var config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            Action act = () => new SnowflakeNumberGenerator(config);

            act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("NodeId must be between 0 and 1023 (Parameter 'nodeId')");
        }

        [Test]
        public void Generate_ShouldReturn_19DigitString()
        {
            var result = generator.Generate();

            result.Length.Should().Be(19);
        }

        [Test]
        public void Generate_ShouldReturn_NumericString()
        {
            var result = generator.Generate();

            long.TryParse(result, out _).Should().BeTrue();
        }

        [Test]
        public void Generate_ShouldProduceUniqueNumbers_WhenCalledMultipleTimes()
        {
            var results = Enumerable.Range(0, 1000).Select(_ => generator.Generate()).ToList();

            results.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void Generate_ShouldBeThreadSafe()
        {
            var tasks = Enumerable.Range(0, 1000)
                                  .Select(_ => Task.Run(() => generator.Generate()))
                                  .ToArray();

            Task.WaitAll(tasks);

            var results = tasks.Select(x => x.Result).ToList();
            results.Should().OnlyHaveUniqueItems();
            results.Count.Should().Be(1000);
        }

        [Test]
        public void Generate_ShouldProduceSequentialNumbers()
        {
            var first = generator.Generate();
            var second = generator.Generate();
            var third = generator.Generate();

            long.Parse(second).Should().Be(long.Parse(first) + 1);
            long.Parse(third).Should().Be(long.Parse(second) + 1);
        }

        [Test]
        public void Generate_ShouldProduceGloballyUniqueNumbers_MultiThreaded()
        {
            int totalTasks = 5000;
            var tasks = new Task<string>[totalTasks];

            for (int i = 0; i < totalTasks; i++)
            {
                tasks[i] = Task.Run(generator.Generate);
            }

            Task.WaitAll(tasks);

            var results = tasks.Select(x => x.Result).ToList();

            results.Should().OnlyHaveUniqueItems();
            results.Count.Should().Be(totalTasks);
        }

        [Test]
        public void Generate_ShouldProduceGloballyUniqueNumbers_MultipleGenerators()
        {
            int totalGenerators = 5;
            int perGenerator = 1000;

            var generators = Enumerable.Range(1, totalGenerators)
                .Select(nodeId =>
                {
                    var settings = new Dictionary<string, string>
                    {
                        { "Snowflake:Epoch", "2020-01-01T00:00:00Z" },
                        { "Snowflake:NodeId", nodeId.ToString() }
                    };
                    var cfg = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
                    return new SnowflakeNumberGenerator(cfg);
                }).ToList();

            var results = new List<string>();

            foreach (var gen in generators)
            {
                for (int i = 0; i < perGenerator; i++)
                {
                    results.Add(gen.Generate());
                }
            }

            results.Should().OnlyHaveUniqueItems();
            results.Count.Should().Be(totalGenerators * perGenerator);
        }
    }
}
 
