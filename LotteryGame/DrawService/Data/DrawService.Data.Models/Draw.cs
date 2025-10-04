namespace DrawService.Data.Models
{
    public class Draw : BaseEntity
    {
        public DateTime DrawDate { get; set; }

        public DrawStatus Status { get; set; }

        public long TicketPriceInCents { get; set; }

        public long HouseProfit { get; set; }

        public ICollection<string> PrizeIds { get; set; } = new HashSet<string>();

        public IDictionary<int, ICollection<string>> PlayerTickets { get; set; } = new Dictionary<int, ICollection<string>>();
    }
}
