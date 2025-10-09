namespace WagerService.Core.Validation.Contexts
{
    using WagerService.Data.Models;

    public class UpdateTicketOperationContext
    {
        public IEnumerable<string> TicketIds { get; set; }

        public IEnumerable<Ticket> Tickets { get; set; }
    }
}
