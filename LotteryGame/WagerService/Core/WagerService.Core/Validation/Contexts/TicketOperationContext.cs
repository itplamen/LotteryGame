namespace WagerService.Core.Validation.Contexts
{
    using WagerService.Data.Models;

    public class TicketOperationContext
    {
        public int NumberOfTickets { get; set; }

        public IEnumerable<string> TicketIds { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }
    }
}
