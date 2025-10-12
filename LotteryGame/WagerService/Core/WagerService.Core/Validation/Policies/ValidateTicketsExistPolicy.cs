namespace WagerService.Core.Validation.Policies
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    using WagerService.Core.Validation.Contexts;
    using WagerService.Data.Contracts;
    using WagerService.Data.Models;

    public class ValidateTicketsExistPolicy : IOperationPolicy<BaseTicketOperationContext>
    {
        private readonly IRepository<Ticket> repository;

        public ValidateTicketsExistPolicy(IRepository<Ticket> repository)
        {
            this.repository = repository;
        }

        public async Task<ResponseDto> ExecuteAsync(BaseTicketOperationContext context)
        {
            if (context.TicketIds == null || !context.TicketIds.Any())
            {
                return new ResponseDto("No ticket ids provided");
            }

            var tickets = await repository.FindAsync(x => context.TicketIds.Contains(x.Id));

            if (tickets == null || !tickets.Any())
            {
                return new ResponseDto("Tickets not found");
            }

            context.Tickets = tickets;
            return new ResponseDto();
        }
    }
}
