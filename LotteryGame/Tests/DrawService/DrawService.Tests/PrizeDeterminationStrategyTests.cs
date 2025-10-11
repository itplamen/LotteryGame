namespace DrawService.Tests
{
    using FluentAssertions;

    using Moq;

    using DrawService.Core.Contracts;
    using DrawService.Core.PrizeDeterminations;
    using DrawService.Data.Models;

    [TestFixture]
    public class PrizeDeterminationStrategyTests
    {
        private Mock<IPrizeStrategy> grandPrizeMock;
        private Mock<IPrizeStrategy> secondTierMock;
        private Mock<IPrizeStrategy> thirdTierMock;
        private PrizeDeterminationStrategy strategy;

        private Draw draw;

        [SetUp]
        public void Setup()
        {
            draw = new Draw
            {
                Id = "draw1",
                TicketPriceInCents = 100,
                PlayerTickets = new List<PlayerTicketInfo>()
                {
                    new PlayerTicketInfo()
                    {
                        PlayerId = 1,
                        Tickets = new List<string> { "t1", "t2", "t3" }
                    },
                    new PlayerTicketInfo()
                    {
                        PlayerId = 2,
                        Tickets = new List<string> { "t4", "t5", "t6" }
                    }
                }
            };

            grandPrizeMock = new Mock<IPrizeStrategy>();
            secondTierMock = new Mock<IPrizeStrategy>();
            thirdTierMock = new Mock<IPrizeStrategy>();

            strategy = new PrizeDeterminationStrategy(
                new List<IPrizeStrategy>()
                {
                    grandPrizeMock.Object,
                    secondTierMock.Object,
                    thirdTierMock.Object
                });
        }

        [Test]
        public void DeterminePrizes_ShouldReturnEmpty_WhenNoTickets()
        {
            draw.PlayerTickets.Clear();

            var result = strategy.DeterminePrizes(draw);

            result.Should().BeEmpty();
            draw.HouseProfit.Should().Be(0);
        }

        [Test]
        public void DeterminePrizes_ShouldCallEachStrategy()
        {
            var allTickets = draw.PlayerTickets.SelectMany(x => x.Tickets).ToList();
            long totalRevenue = allTickets.Count * draw.TicketPriceInCents;

            grandPrizeMock
                .Setup(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue))
                .Returns(new List<Prize> { new Prize { TicketId = "t1", AmountInCents = 300 } });

            secondTierMock
                .Setup(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue))
                .Returns(new List<Prize> { new Prize { TicketId = "t2", AmountInCents = 200 } });

            thirdTierMock
                .Setup(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue))
                .Returns(new List<Prize> { new Prize { TicketId = "t3", AmountInCents = 100 } });

            var result = strategy.DeterminePrizes(draw).ToList();

            grandPrizeMock.Verify(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue), Times.Once);
            secondTierMock.Verify(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue), Times.Once);
            thirdTierMock.Verify(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue), Times.Once);

            result.Should().HaveCount(3);
            result.Select(x => x.TicketId).Should().BeEquivalentTo(new List<string>() { "t1", "t2", "t3" });
        }

        [Test]
        public void DeterminePrizes_ShouldRemoveWinningTicketsBetweenTiers()
        {
            var allTickets = draw.PlayerTickets.SelectMany(x => x.Tickets).ToList();
            long totalRevenue = allTickets.Count * draw.TicketPriceInCents;

            grandPrizeMock
                .Setup(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue))
                .Returns(new List<Prize> { new Prize { TicketId = "t1", AmountInCents = 300 } });

            secondTierMock
                .Setup(x => x.Calculate(draw,
                    It.Is<List<string>>(ids => !ids.Contains("t1")),
                    totalRevenue))
                .Returns(new List<Prize> { new Prize { TicketId = "t2", AmountInCents = 200 } });

            var result = strategy.DeterminePrizes(draw).ToList();

            secondTierMock.VerifyAll();
        }

        [Test]
        public void DeterminePrizes_ShouldCalculateHouseProfitCorrectly()
        {
            var allTickets = draw.PlayerTickets.SelectMany(x => x.Tickets).ToList();
            long totalRevenue = allTickets.Count * draw.TicketPriceInCents;

            grandPrizeMock
                .Setup(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue))
                .Returns(new List<Prize> { new Prize { TicketId = "t1", AmountInCents = 300 } });

            secondTierMock
                .Setup(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue))
                .Returns(new List<Prize> { new Prize { TicketId = "t2", AmountInCents = 200 } });

            thirdTierMock
                .Setup(x => x.Calculate(draw, It.IsAny<List<string>>(), totalRevenue))
                .Returns(new List<Prize> { new Prize { TicketId = "t3", AmountInCents = 100 } });

            strategy.DeterminePrizes(draw);

            draw.HouseProfit.Should().Be(totalRevenue - (300 + 200 + 100));
        }
    }
}
