namespace DrawService.Core.Validation.Contexts
{
    public class DrawOperationContext : BaseDrawContext
    {
        public int PlayerId { get; set; }

        public IEnumerable<string> TicketIds { get; set; }

        public int MinTicketsPerPlayer { get; set; }

        public int MaxTicketsPerPlayer { get; set; }
    }
}
