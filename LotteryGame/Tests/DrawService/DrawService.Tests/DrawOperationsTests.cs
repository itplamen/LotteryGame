namespace DrawService.Tests
{
    using AutoMapper;

    using FluentAssertions;

    using Moq;

    using Microsoft.Extensions.Configuration;

    using DrawService.Core.Models;
    using DrawService.Core.Operations;
    using DrawService.Core.Validation.Contexts;
    using DrawService.Core.Validation.Policies;
    using DrawService.Data.Models;
    using DrawService.Tests.Utils.Database;
    using LotteryGame.Common.Utils.Validation;    
    
    [TestFixture]
    public class DrawOperationsTests
    {
        private Mock<IMapper> mapperMock;
        private IConfiguration configuration;
        private DbRepositoryMock<Draw> drawRepositoryMock;
        private DrawOperations drawOperations;
        private IDictionary<string, string> inMemoryConfig;

        [SetUp]
        public void Setup()
        {
            mapperMock = new Mock<IMapper>();

            inMemoryConfig = new Dictionary<string, string>()
            {
                { "TicketPriceInCents", "100" },
                { "DrawScheduleTime", "1000" },
                { "MinTicketsPerPlayer", "2" },
                { "MaxTicketsPerPlayer", "5" },
                { "MinPlayersInDraw", "2" },
                { "MaxPlayersInDraw", "5" }
            };
            configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemoryConfig).Build();

            drawRepositoryMock = new DbRepositoryMock<Draw>();

            var joinDrawPolicies = new List<IOperationPolicy<JoinDrawOperationContext>>()
            {
                new DrawMustExistPolicy<JoinDrawOperationContext>(drawRepositoryMock.Mock),
                new DrawInValidStatusPolicy<JoinDrawOperationContext>(),
                new PlayerNotAlreadyJoinedPolicy(),
                new DrawCapacityPolicy(),
                new TicketsCountPolicy(),
            };

            var joinDrawPipeline = new OperationPipeline<JoinDrawOperationContext>(joinDrawPolicies);

            var startDrawPolicies = new List<IOperationPolicy<StartDrawOperationContext>>()
            {
                new DrawMustExistPolicy<StartDrawOperationContext>(drawRepositoryMock.Mock),
                new DrawInValidStatusPolicy<StartDrawOperationContext>(),
                new DrawStartPolicy()
            };

            var startDrawPipeline = new OperationPipeline<StartDrawOperationContext>(startDrawPolicies);

            drawOperations = new DrawOperations(mapperMock.Object, drawRepositoryMock.Mock, configuration, joinDrawPipeline, startDrawPipeline);
        }

        [Test]
        public async Task GetOpenDraw_ShouldReturnError_WhenNoPendingDraws()
        {
            var result = await drawOperations.GetOpenDraw(1);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("No open draws available");
        }

        [Test]
        public async Task GetOpenDraw_ShouldReturnDraw_WhenPendingAndPlayerNotJoined()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Pending,
                PlayerTickets = new Dictionary<int, ICollection<string>>()
            };
            drawRepositoryMock.Data.Add(draw);

            mapperMock.Setup(m => m.Map<DrawDto>(It.IsAny<Draw>()))
                      .Returns(new DrawDto { Id = draw.Id });

            var result = await drawOperations.GetOpenDraw(1);

            result.IsSuccess.Should().BeTrue();
            result.Data.Id.Should().Be("draw1");
            result.Data.MinTicketsPerPlayer.Should().Be(int.Parse(inMemoryConfig["MinTicketsPerPlayer"]));
            result.Data.MaxTicketsPerPlayer.Should().Be(int.Parse(inMemoryConfig["MaxTicketsPerPlayer"]));
            result.Data.MinPlayersInDraw.Should().Be(int.Parse(inMemoryConfig["MinPlayersInDraw"]));
            result.Data.MaxPlayersInDraw.Should().Be(int.Parse(inMemoryConfig["MaxPlayersInDraw"]));
        }

        [Test]
        public async Task GetDrawsForSettlement_ShouldReturnIdsOfInProgressDraws()
        {
            drawRepositoryMock.Data.AddRange(new List<Draw>()
            {
                new Draw { Id = "d1", Status = DrawStatus.InProgress },
                new Draw { Id = "d2", Status = DrawStatus.Completed }
            });

            var result = await drawOperations.GetDrawsForSettlement();

            result.Should().ContainSingle(x => x == "d1");
        }

        [Test]
        public async Task Create_ShouldAddPendingDraw_WithConfiguredValues()
        {
            mapperMock.Setup(x => x.Map<DrawDto>(It.IsAny<Draw>()))
                      .Returns<Draw>(x => new DrawDto()
                      { 
                          Id = x.Id, 
                          Status = x.Status,
                          MaxTicketsPerPlayer = x.MaxTicketsPerPlayer,
                          MinTicketsPerPlayer = x.MinTicketsPerPlayer,
                          MaxPlayersInDraw = x.MaxPlayersInDraw,
                          MinPlayersInDraw = x.MinPlayersInDraw
                      });

            var result = await drawOperations.Create();

            result.IsSuccess.Should().BeTrue();
            result.Data.Status.Should().Be(DrawStatus.Pending);

            var stored = drawRepositoryMock.Data.First();
            stored.TicketPriceInCents.Should().Be(int.Parse(inMemoryConfig["TicketPriceInCents"]));
            result.Data.MinTicketsPerPlayer.Should().Be(int.Parse(inMemoryConfig["MinTicketsPerPlayer"]));
            result.Data.MaxTicketsPerPlayer.Should().Be(int.Parse(inMemoryConfig["MaxTicketsPerPlayer"]));
            result.Data.MinPlayersInDraw.Should().Be(int.Parse(inMemoryConfig["MinPlayersInDraw"]));
            result.Data.MaxPlayersInDraw.Should().Be(int.Parse(inMemoryConfig["MaxPlayersInDraw"]));
        }

        [Test]
        public async Task Join_ShouldReturnError_WhenDrawNotFound()
        {
            var result = await drawOperations.Join("missing", 1, new List<string>() { "t1" });

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Draw not found");
        }

        [Test]
        public async Task Join_ShouldReturnError_WhenStatusIsNotPending()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Completed,
                PlayerTickets = new Dictionary<int, ICollection<string>>()
            };
            drawRepositoryMock.Data.Add(draw);

            var result = await drawOperations.Join("draw1", 1, new List<string>() { "t1" });

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Invalid draw status");
        }

        [Test]
        public async Task Join_ShouldReturnError_WhenPlayerAlreadyJoined()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Pending,
                PlayerTickets = new Dictionary<int, ICollection<string>> { [1] = new List<string> { "t1" } }
            };
            drawRepositoryMock.Data.Add(draw);

            var result = await drawOperations.Join("draw1", 1, new List<string>() { "t2" });

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Player already joined the draw");
        }

        [Test]
        public async Task Join_ShouldReturnError_WhenDrawIsFull()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Pending,
                MaxPlayersInDraw = 1,
                PlayerTickets = new Dictionary<int, ICollection<string>> { [1] = new List<string> { "t1" } }
            };
            drawRepositoryMock.Data.Add(draw);

            var result = await drawOperations.Join("draw1", 2, new List<string>() { "t2" });

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Draw is full");
        }

        [Test]
        public async Task Join_ShouldReturnError_WhenInvalidNumberOfTickets()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Pending,
                MaxPlayersInDraw = 1,
                PlayerTickets = new Dictionary<int, ICollection<string>>()
            };
            drawRepositoryMock.Data.Add(draw);

            var result = await drawOperations.Join("draw1", 1, new List<string>() { });

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Contain($"Invalid number of tickets. Min: {inMemoryConfig["MinTicketsPerPlayer"]}, Max: {inMemoryConfig["MaxTicketsPerPlayer"]}");
        }

        [Test]
        public async Task Join_ShouldAddPlayer_WhenValid()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Pending,
                MaxPlayersInDraw = 1,
                PlayerTickets = new Dictionary<int, ICollection<string>>()
            };
            
            drawRepositoryMock.Data.Add(draw);

            mapperMock.Setup(m => m.Map<DrawDto>(It.IsAny<Draw>()))
                      .Returns(new DrawDto { Id = "draw1" });

            var result = await drawOperations.Join("draw1", 1, new List<string>() { "t1", "t2" });

            result.IsSuccess.Should().BeTrue();
            drawRepositoryMock.Data.First().PlayerTickets.Should().ContainKey(1);
        }

        [Test]
        public async Task Start_ShouldReturnError_WhenDrawNotFound()
        {
            var result = await drawOperations.Start("missing");

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Draw not found");
        }

        [Test]
        public async Task Start_ShouldReturnError_WhenInvalidStatus()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Completed,
                PlayerTickets = new Dictionary<int, ICollection<string>>()
            };
            drawRepositoryMock.Data.Add(draw);

            var result = await drawOperations.Start("draw1");

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Invalid draw status");
        }

        [Test]
        public async Task Start_ShouldReturnError_WhenNotEnoughPlayers()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Pending,
                MinPlayersInDraw = 11,
                MaxPlayersInDraw = 13,
                MinTicketsPerPlayer = int.Parse(inMemoryConfig["MinTicketsPerPlayer"]),
                MaxTicketsPerPlayer = int.Parse(inMemoryConfig["MaxTicketsPerPlayer"]),
                PlayerTickets = new Dictionary<int, ICollection<string>>() { [1] = new List<string> { "t1" } }
            };
            drawRepositoryMock.Data.Add(draw);

            var result = await drawOperations.Start("draw1");

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Draw cannot be started");
        }

        [Test]
        public async Task Start_ShouldUpdateStatus_WhenValid()
        {
            var draw = new Draw()
            {
                Id = "draw1",
                Status = DrawStatus.Pending,
                PlayerTickets = new Dictionary<int, ICollection<string>>()
                {
                    [1] = new List<string> { "t1" },
                    [2] = new List<string> { "t2" }
                }
            };
            drawRepositoryMock.Data.Add(draw);

            var result = await drawOperations.Start("draw1");

            result.IsSuccess.Should().BeTrue();
            drawRepositoryMock.Data.First().Status.Should().Be(DrawStatus.InProgress);
        }
    }
}
