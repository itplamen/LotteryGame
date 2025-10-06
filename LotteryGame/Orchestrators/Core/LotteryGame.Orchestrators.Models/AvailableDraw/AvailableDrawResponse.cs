namespace LotteryGame.Orchestrators.Models.AvailableDraw
{
    public class AvailableDrawResponse
    {
        public string DrawId { get; set; }

        public DateTime DrawDate { get; set; }

        public string Status { get; set; }

        public long TicketPriceInCents { get; set; }

        public int MinTicketsPerPlayer { get; set; }

        public int MaxTicketsPerPlayer { get; set; }

        public int MinPlayersInDraw { get; set; }

        public int MaxPlayersInDraw { get; set; }

        public int CurrentPlayersInDraw { get; set; }
    }
}
