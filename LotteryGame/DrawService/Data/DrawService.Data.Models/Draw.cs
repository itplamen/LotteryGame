namespace DrawService.Data.Models
{
    using Newtonsoft.Json;

    public class Draw : BaseEntity
    {
        public DateTime DrawDate { get; set; }

        public DrawStatus Status { get; set; }

        public long TicketPriceInCents { get; set; }

        public int MinTicketsPerPlayer { get; set; }

        public int MaxTicketsPerPlayer { get; set; }

        public int MinPlayersInDraw { get; set; }

        public int MaxPlayersInDraw { get; set; }

        public long HouseProfit { get; set; }

        public ICollection<string> PrizeIds { get; set; } = new HashSet<string>();

        public IDictionary<int, ICollection<string>> PlayerTickets { get; set; } = new Dictionary<int, ICollection<string>>();

        [JsonIgnore]
        public int CurrentPlayersInDraw => PlayerTickets.Keys.Count;
    }
}
