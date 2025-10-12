namespace LotteryGame.Clients.Core.Services.Models.History
{
    public class PrizeHistory
    {
        public string TicketNumber { get; set; }

        public decimal Amount { get; set; }

        public string Tier { get; set; }

        public string Status { get; set; }

        public int PlayerId { get; set; }
    }
}
