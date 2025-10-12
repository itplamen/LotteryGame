namespace WagerService.Core.Operations
{
    using AutoMapper;
   
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WagerService.Core.Contracts;
    using WagerService.Core.Models;
    using WagerService.Core.Validation.Contexts;
    using WagerService.Data.Contracts;
    using WagerService.Data.Models;

    public class TicketOperations : ITicketOperations
    {
        private readonly IMapper mapper;
        private readonly IRepository<Ticket> repository;
        private readonly INumberGeneration numberGeneration;
        private readonly OperationPipeline<CreateTicketOperationContext> createPipeline;
        private readonly OperationPipeline<BaseTicketOperationContext> updatePipeline;

        public TicketOperations(
            IMapper mapper,
            IRepository<Ticket> repository,
            INumberGeneration numberGeneration,
            OperationPipeline<CreateTicketOperationContext> createPipeline,
            OperationPipeline<BaseTicketOperationContext> updatePipeline)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.numberGeneration = numberGeneration;
            this.createPipeline = createPipeline;
            this.updatePipeline = updatePipeline;
        }

        public async Task<ResponseDto<IEnumerable<TicketDto>>> Create(TicketCreateRequestDto request)
        {
            var context = new CreateTicketOperationContext()
            {
                NumberOfTickets = request.NumberOfTickets
            };

            ResponseDto validationResult = await createPipeline.ExecuteAsync(context);
            if (!validationResult.IsSuccess)
            {
                return new ResponseDto<IEnumerable<TicketDto>> { ErrorMsg = validationResult.ErrorMsg };
            }

            IEnumerable<Ticket> tickets = Enumerable.Range(0, request.NumberOfTickets)
                .Select(_ => new Ticket()
                {
                    TicketNumber = numberGeneration.Generate(),
                    PlayerId = request.PlayerId,
                    DrawId = request.DrawId,
                    ReservationId = request.ReservationId,
                    Status = TicketStatus.Pending
                })
                .ToList();

            IEnumerable<Ticket> created = await repository.AddAsync(tickets);

            return new ResponseDto<IEnumerable<TicketDto>>() { Data = mapper.Map<IEnumerable<TicketDto>>(created) };
        }

        public async Task<ResponseDto<IEnumerable<TicketDto>>> Update(TicketUpdateRequestDto request)
        {
            var context = new BaseTicketOperationContext()
            {
                TicketIds = request.TicketIds
            };

            ResponseDto validationResult = await updatePipeline.ExecuteAsync(context);
            if (!validationResult.IsSuccess)
            {
                return new ResponseDto<IEnumerable<TicketDto>> { ErrorMsg = validationResult.ErrorMsg };
            }

            IEnumerable<Ticket> updatedTickets = context.Tickets.Select(x =>
            {
                x.Status = request.Status;
                return x;
            }).ToList();

            await repository.UpdateAsync(updatedTickets);

            return new ResponseDto<IEnumerable<TicketDto>>() 
            { 
                Data = mapper.Map<IEnumerable<TicketDto>>(updatedTickets) 
            };
        }
    }
}
