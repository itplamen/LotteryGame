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

        public async Task<ResponseDto<TicketDto>> Create(int playerId, string drawId, decimal amount, int reservationId)
        {
            if (amount <= 0)
            {
                return new ResponseDto<TicketDto>("Invalid ticket amount");
            }

            string ticketNumber = numberGeneration.Generate();

            var ticket = new Ticket()
            {
                TicketNumber = ticketNumber,
                PlayerId = playerId,
                DrawId = drawId,
                Amount = amount,
                ReservationId = reservationId,
                Status = TicketStatus.Pending
            };

            await repository.AddAsync(ticket);

            return new ResponseDto<TicketDto>() { Data = mapper.Map<TicketDto>(ticket) };
        }

        public async Task<ResponseDto<TicketDto>> Update(string ticketId, TicketStatus status)
        {
            Ticket ticket = await repository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                return new ResponseDto<TicketDto>("Ticket not found");
            }

            ticket.Status = status;

            bool success = await repository.UpdateAsync(ticket);

            if (!success)
            {
                return new ResponseDto<TicketDto>("Unsucessful ticket update");
            }

            return new ResponseDto<TicketDto>() { Data = mapper.Map<TicketDto>(ticket) };
        }
    }
}
