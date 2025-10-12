namespace LotteryGame.Clients.Core.Services.Models
{
    public class PrizeHistory
    {
        public string TicketNumber { get; set; }

        public decimal Amount { get; set; }

        public string Tier { get; set; }

        public string Status { get; set; }
    }
}
