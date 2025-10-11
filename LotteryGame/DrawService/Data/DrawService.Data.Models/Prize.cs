namespace DrawService.Data.Models
{
    public class Prize : BaseEntity
    {
        public string TicketId { get; set; }

        public long AmountInCents { get; set; }

        public PrizeTier Tier { get; set; }

        public string DrawId { get; set; }
    }
}
