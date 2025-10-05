namespace WagerService.Core.Operations
{
    using AutoMapper;
   
    using LotteryGame.Common.Models.Dto;
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

        public async Task<ResponseDto<IEnumerable<TicketDto>>> Create(TicketCreateRequestDto request)
        {
            if (request.NumberOfTickets < 0)
            {
                return new ResponseDto<IEnumerable<TicketDto>>()
                {
                    ErrorMsg = "Invalid number of tickets to create"
                };
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
            if (request.TicketIds == null || !request.TicketIds.Any())
            {
                return new ResponseDto<IEnumerable<TicketDto>>("No ticket ids provided");
            }

            IEnumerable<Ticket> tickets = await repository.FindAsync(x => request.TicketIds.Contains(x.Id));

            if (tickets == null)
            {
                return new ResponseDto<IEnumerable<TicketDto>>("Tickets not found");
            }

            IEnumerable<Ticket> updatedTickets = tickets.Select(ticket =>
            {
                ticket.Status = request.Status;
                return ticket;
            });

            bool success = await repository.UpdateAsync(updatedTickets);

            if (!success)
            {
                return new ResponseDto<IEnumerable<TicketDto>>("Unsucessful ticket update");
            }

            return new ResponseDto<IEnumerable<TicketDto>>() 
            { 
                Data = mapper.Map<IEnumerable<TicketDto>>(updatedTickets) 
            };
        }
    }
}
