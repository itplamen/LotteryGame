namespace DrawService.Tests
{
    using FluentAssertions;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using DrawService.Core.PrizeDeterminations;
    using DrawService.Data.Models;
    
    [TestFixture]
    public class ThirdTierPrizeStrategyTests
    {
        private Mock<IConfiguration> configurationMock;
        private ThirdTierPrizeStrategy strategy;

        [SetUp]
        public void Setup()
        {
            configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Prize:ThirdPrizeRevenueShare"]).Returns("0.1");
            configurationMock.Setup(x => x["Prize:ThirdPrizeTicketShare"]).Returns("0.2");

            strategy = new ThirdTierPrizeStrategy(configurationMock.Object);
        }

        [Test]
        public void Calculate_ShouldReturnEmpty_WhenNoTickets()
        {
            var draw = new Draw { Id = "draw2" };
            var prizes = strategy.Calculate(draw, new List<string>(), 1000);

            prizes.Should().BeEmpty();
        }

        [Test]
        public void Calculate_ShouldCreatePrizes_WithCorrectTierAndAmounts()
        {
            var draw = new Draw { Id = "draw2" };
            var remainingTickets = new List<string> { "t1", "t2", "t3", "t4", "t5" };
            long totalRevenue = 1000;

            var prizes = strategy.Calculate(draw, remainingTickets, totalRevenue).ToList();

            prizes.Should().NotBeEmpty();
            prizes.Should().OnlyContain(p => p.Tier == PrizeTier.Third);
            prizes.Should().OnlyContain(p => p.DrawId == draw.Id);
            prizes.Should().OnlyContain(p => remainingTickets.Contains(p.TicketId));

            var expectedWinnerCount = (int)Math.Round(remainingTickets.Count * 0.2m);
            expectedWinnerCount.Should().BeGreaterThanOrEqualTo(1);
            prizes.Should().HaveCount(expectedWinnerCount);

            var expectedAmount = (long)(totalRevenue * 0.1m / expectedWinnerCount);
            prizes.Should().OnlyContain(p => p.AmountInCents == expectedAmount);
        }

        [Test]
        public void Calculate_ShouldSelectDifferentWinners_OverMultipleDraws()
        {
            var draw = new Draw { Id = "draw2" };
            var tickets = new List<string> { "t1", "t2", "t3", "t4", "t5" };

            var winners = new HashSet<string>();
            for (int i = 0; i < 50; i++)
            {
                var prize = strategy.Calculate(draw, tickets, 1000).First();
                winners.Add(prize.TicketId);
            }

            winners.Should().BeSubsetOf(tickets);
            winners.Count.Should().BeGreaterThan(1);
        }
    }
}
