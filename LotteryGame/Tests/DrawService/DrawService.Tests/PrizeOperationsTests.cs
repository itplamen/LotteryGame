namespace DrawService.Tests
{
    using AutoMapper;

    using FluentAssertions;

    using Moq;

    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using DrawService.Core.Operations;
    using DrawService.Core.Validation.Contexts;
    using DrawService.Core.Validation.Policies;
    using DrawService.Data.Models;
    using DrawService.Tests.Utils.Database;
    using DrawService.Tests.Utils.Stubs;
    using LotteryGame.Common.Utils.Validation;

    [TestFixture]
    public class PrizeOperationsTests
    {
        private Mock<IMapper> mapperMock;
        private DbRepositoryMock<Draw> drawRepositoryMock;
        private DbRepositoryMock<Prize> prizeRepositoryMock;
        private Mock<IPrizeDeterminationStrategy> prizeDeterminationStrategyMock;
        private OperationPipeline<PrizeOperationContext> operationPipeline;
        private PrizeOperations prizeOperations;

        [SetUp]
        public void Setup()
        {
            mapperMock = new Mock<IMapper>();
            drawRepositoryMock = new DbRepositoryMock<Draw>();
            prizeRepositoryMock = new DbRepositoryMock<Prize>();
            prizeDeterminationStrategyMock = new Mock<IPrizeDeterminationStrategy>();

            var policies = new List<IOperationPolicy<PrizeOperationContext>>()
            {
                new DrawMustExistPolicy<PrizeOperationContext>(drawRepositoryMock.Mock),
                new DrawInValidStatusPolicy<PrizeOperationContext>(),
                new DrawHasTicketsPolicy()
            };

            operationPipeline = new OperationPipeline<PrizeOperationContext>(policies);

            prizeOperations = new PrizeOperations(
                mapperMock.Object,
                drawRepositoryMock.Mock,
                prizeRepositoryMock.Mock,
                prizeDeterminationStrategyMock.Object,
                operationPipeline
            );
        }

        [Test]
        public async Task DeterminePrizes_ShouldReturnError_WhenDrawNotFound()
        {
            var result = await prizeOperations.DeterminePrizes("draw1");

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Draw not found");
        }

        [Test]
        public async Task DeterminePrizes_ShouldReturnError_WhenDrawStatusIsNotInProgress()
        {
            drawRepositoryMock.SetupGetById(DrawsStub.CompletedDraw.Id, DrawsStub.CompletedDraw);

            var result = await prizeOperations.DeterminePrizes(DrawsStub.CompletedDraw.Id);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Invalid draw status");
        }

        [Test]
        public async Task DeterminePrizes_ShouldReturnError_WhenNoTicketsExist()
        {
            drawRepositoryMock.SetupGetById(DrawsStub.InProgressDraWithoutTickets.Id, DrawsStub.InProgressDraWithoutTickets);

            var result = await prizeOperations.DeterminePrizes(DrawsStub.InProgressDraWithoutTickets.Id);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("No tickets for draw");
        }

        [Test]
        public async Task DeterminePrizes_ShouldDetermineAndPersistPrizes_WhenDrawIsValid()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.InProgress,
                PlayerTickets = new Dictionary<int, ICollection<string>>
                {
                    { 1, new List<string> { "t1", "t2" } },
                    { 2, new List<string> { "t3" } }
                }
            };

            var prizes = new List<Prize>()
            {
                new Prize { Id = "p1", TicketId = "t1", Amount = 100, Tier = PrizeTier.Grand },
                new Prize { Id = "p2", TicketId = "t2", Amount = 50, Tier = PrizeTier.Second }
            };

            var prizeDtos = new List<PrizeDto>()
            {
                new PrizeDto { Id = "p1" },
                new PrizeDto { Id = "p2" }
            };

            drawRepositoryMock.SetupGetById("draw1", draw);


            prizeDeterminationStrategyMock.Setup(s => s.DeterminePrizes(draw)).Returns(prizes);
            mapperMock.Setup(m => m.Map<IEnumerable<PrizeDto>>(prizes)).Returns(prizeDtos);

            var result = await prizeOperations.DeterminePrizes("draw1");

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(prizeDtos);

            draw.PrizeIds.Should().Contain(new[] { "p1", "p2" });
            draw.Status.Should().Be(DrawStatus.Completed);
        }
    }
}
