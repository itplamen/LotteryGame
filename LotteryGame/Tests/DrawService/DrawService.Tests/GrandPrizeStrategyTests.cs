namespace DrawService.Tests
{
    using FluentAssertions;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using DrawService.Core.PrizeDeterminations;
    using DrawService.Data.Models;

    [TestFixture]
    public class GrandPrizeStrategyTests
    {
        private Mock<IConfiguration> configurationMock;
        private GrandPrizeStrategy strategy;

        [SetUp]
        public void Setup()
        {
            configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Prize:GrandPrizeRevenueShare"]).Returns("0.5");

            strategy = new GrandPrizeStrategy(configurationMock.Object);
        }

        [Test]
        public void Calculate_ShouldReturnEmpty_WhenNoTickets()
        {
            var draw = new Draw { Id = "draw1" };
            List<string> remainingTickets = new List<string>();
            long totalRevenue = 1000;

            var prizes = strategy.Calculate(draw, remainingTickets, totalRevenue);

            prizes.Should().BeEmpty();
        }

        [Test]
        public void Calculate_ShouldReturnSingleGrandPrize_WhenTicketsExist()
        {
            var draw = new Draw { Id = "draw1" };
            var remainingTickets = new List<string> { "t1", "t2", "t3" };
            long totalRevenue = 1000;

            var prizes = strategy.Calculate(draw, remainingTickets, totalRevenue).ToList();

            prizes.Should().HaveCount(1);

            var prize = prizes.First();
            prize.DrawId.Should().Be(draw.Id);
            remainingTickets.Should().Contain(prize.TicketId);
            prize.AmountInCents.Should().Be((long)(totalRevenue * 0.5m));
            prize.Tier.Should().Be(PrizeTier.GrandPrize);
        }

        [Test]
        public void Calculate_ShouldRandomlySelectDifferentWinners_OnMultipleCalls()
        {
            var draw = new Draw { Id = "draw1" };
            var remainingTickets = new List<string> { "t1", "t2", "t3", "t4", "t5" };
            long totalRevenue = 1000;

            var winners = new HashSet<string>();

            for (int i = 0; i < 50; i++)
            {
                var prize = strategy.Calculate(draw, remainingTickets, totalRevenue).First();
                winners.Add(prize.TicketId);
            }

            winners.Should().BeSubsetOf(remainingTickets);
            winners.Count.Should().BeGreaterThan(1);
        }
    }
}