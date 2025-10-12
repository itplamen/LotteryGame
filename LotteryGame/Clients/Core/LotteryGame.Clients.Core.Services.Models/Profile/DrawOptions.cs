namespace LotteryGame.Clients.Core.Services.Models.Profile
{
    public class DrawOptions
    {
        public decimal TicketPrice { get; set; }

        public int MinTicketsPerPlayer { get; set; }

        public int MaxTicketsPerPlayer { get; set; }

        public int MinPlayersInDraw { get; set; }

        public int MaxPlayersInDraw { get; set; }
    }
}
