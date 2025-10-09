namespace WagerService.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using FluentAssertions;
    
    using LotteryGame.Common.Utils.Validation;
    using WagerService.Core.Models;
    using WagerService.Core.Operations;
    using WagerService.Core.Validation.Contexts;
    using WagerService.Core.Validation.Policies;
    using WagerService.Data.Models;
    using WagerService.Tests.Utils.Database;
    using WagerService.Tests.Utils.Mapper;
    using WagerService.Tests.Utils.NumberGenerator;

    [TestFixture]
    public class TicketOperationsTests
    {
        private MapperMock mapperMock;
        private DbRepositoryMock<Ticket> repositoryMock;
        private NumberGeneratorMock numberGenerationMock;
        private TicketOperations operations;

        [SetUp]
        public void Setup()
        {
            mapperMock = new MapperMock();
            repositoryMock = new DbRepositoryMock<Ticket>();
            numberGenerationMock = new NumberGeneratorMock();

            var createPolicies = new List<IOperationPolicy<CreateTicketOperationContext>>{new  ValidateNumberOfTicketsPolicy()};
            var createPipeline = new OperationPipeline<CreateTicketOperationContext>(createPolicies);
            var updatePolicies = new List<IOperationPolicy<UpdateTicketOperationContext>>{new  ValidateTicketsExistPolicy(repositoryMock.Mock)};
            var updatePipeline = new OperationPipeline<UpdateTicketOperationContext>(updatePolicies);

            operations = new TicketOperations(
                mapperMock.Mock,
                repositoryMock.Mock,
                numberGenerationMock.Mock,
                createPipeline,
                updatePipeline
            );
        }

        [Test]
        public async Task Create_ShouldReturnError_WhenNumberOfTicketsIsNegative()
        {
            var request = new TicketCreateRequestDto() { NumberOfTickets = -1 };

            var result = await operations.Create(request);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Invalid number of tickets to create");
        }

        [Test]
        public async Task Create_ShouldCreatePendingTickets_WhenValidRequest()
        {
            var request = new TicketCreateRequestDto()
            {
                PlayerId = 1,
                DrawId = "2",
                ReservationId = 3,
                NumberOfTickets = 2
            };

            var result = await operations.Create(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(request.NumberOfTickets);
            result.Data.Should().OnlyContain(x => x.Status == TicketStatus.Pending);
            result.Data.Should().OnlyContain(x => !string.IsNullOrEmpty(x.TicketNumber));
        }

        [Test]
        public async Task Update_ShouldReturnError_WhenTicketIdsAreNullOrEmpty()
        {
            var request = new TicketUpdateRequestDto { TicketIds = null };

            var result = await operations.Update(request);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("No ticket ids provided");

            request.TicketIds = new List<string>();
            result = await operations.Update(request);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("No ticket ids provided");
        }

        [Test]
        public async Task Update_ShouldReturnError_WhenTicketsNotFound()
        {
            var request = new TicketUpdateRequestDto
            {
                TicketIds = new List<string> { "1", "2" },
                Status = TicketStatus.Pending
            };

            var result = await operations.Update(request);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Tickets not found");
        }

        [Test]
        public async Task Update_ShouldSettleTickets_WhenUpdated()
        {
            var request = new TicketCreateRequestDto()
            {
                PlayerId = 1,
                DrawId = "2",
                ReservationId = 3,
                NumberOfTickets = 2
            };

            var resultCreate = await operations.Create(request);

            var updateReq = new TicketUpdateRequestDto()
            {
                TicketIds = resultCreate.Data.Select(x => x.Id).ToList(),
                Status = TicketStatus.Settled
            };

            var result = await operations.Update(updateReq);

            result.IsSuccess.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.Data.Should().OnlyContain(x => x.Status == TicketStatus.Settled);
        }
    }
}
