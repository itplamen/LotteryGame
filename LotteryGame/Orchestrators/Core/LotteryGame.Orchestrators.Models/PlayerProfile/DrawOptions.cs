namespace LotteryGame.Orchestrators.Models.PlayerProfile
{
    public class DrawOptions
    {
        public long TicketPriceInCents { get; set; }

        public int MinTicketsPerPlayer { get; set; }

        public int MaxTicketsPerPlayer { get; set; }

        public int MinPlayersInDraw { get; set; }

        public int MaxPlayersInDraw { get; set; }
    }
}
