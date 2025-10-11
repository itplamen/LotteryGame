namespace LotteryGame.Orchestrators.Models.PlayerProfile
{
    public class PlayerDraw
    {
        public string DrawId { get; set; }

        public DateTime DrawDate { get; set; }

        public string Status { get; set; }

        public long TicketPriceInCents { get; set; }

        public int CurrentPlayersInDraw { get; set; }

        public IEnumerable<PlayerTicket> Tickets { get; set; }
    }
}
