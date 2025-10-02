namespace DrawService.Data.Models
{
    public class Prize : BaseEntity
    {
        public string TicketId { get; set; }

        public int PlayerId { get; set; }

        public long Amount { get; set; }

        public PrizeTier Tier { get; set; }

        public string DrawId { get; set; }
    }
}
