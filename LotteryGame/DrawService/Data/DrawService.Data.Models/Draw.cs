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

        public long HouseProfitInCents { get; set; }

        public ICollection<string> PrizeIds { get; set; } = new HashSet<string>();

        public ICollection<PlayerTicketInfo> PlayerTickets { get; set; } = new HashSet<PlayerTicketInfo>();

        [JsonIgnore]
        public int CurrentPlayersInDraw => PlayerTickets.Count;
    }
}
