namespace LotteryGame.Orchestrators.Models.DrawHistory
{
    public class PrizeHistory
    {
        public string TicketNumber { get; set; }

        public long AmountInCents { get; set; }

        public string Tier { get; set; }

        public string Status { get; set; }

        public int PlayerId { get; set; }
    }
}
