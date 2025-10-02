namespace WagerService.Core.Operations
{
    using AutoMapper;

    using WagerService.Core.Contracts;
    using WagerService.Core.Models;
    using WagerService.Data.Contracts;
    using WagerService.Data.Models;

    public class TicketOperations : ITicketOperations
    {
        private readonly IMapper mapper;
        private readonly IRepository<Ticket> repository;
        private readonly INumberGeneration numberGeneration;

        public TicketOperations(IMapper mapper, IRepository<Ticket> repository, INumberGeneration numberGeneration)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.numberGeneration = numberGeneration;
        }

        public async Task<ResponseDto<TicketDto>> Create(TicketCreateRequestDto request)
        {
            if (request.Amount <= 0)
            {
                return new ResponseDto<TicketDto>("Invalid ticket amount");
            }

            string ticketNumber = numberGeneration.Generate();

            var ticket = new Ticket()
            {
                TicketNumber = ticketNumber,
                PlayerId = request.PlayerId,
                DrawId = request.DrawId,
                Amount = request.Amount,
                ReservationId = request.ReservationId,
                Status = TicketStatus.Pending
            };

            await repository.AddAsync(ticket);

            return new ResponseDto<TicketDto>() { Data = mapper.Map<TicketDto>(ticket) };
        }

        public async Task<ResponseDto<TicketDto>> Update(TicketUpdateRequestDto request)
        {
            Ticket ticket = await repository.GetByIdAsync(request.TicketId);

            if (ticket == null)
            {
                return new ResponseDto<TicketDto>("Ticket not found");
            }

            ticket.Status = request.Status;

            bool success = await repository.UpdateAsync(ticket);

            if (!success)
            {
                return new ResponseDto<TicketDto>("Unsucessful ticket update");
            }

            return new ResponseDto<TicketDto>() { Data = mapper.Map<TicketDto>(ticket) };
        }
    }
}
