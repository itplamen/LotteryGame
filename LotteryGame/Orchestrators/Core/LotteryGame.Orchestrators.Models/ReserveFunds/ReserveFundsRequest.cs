namespace LotteryGame.Orchestrators.Models
{
    public class ReserveFundsRequest
    {
        public int PlayerId { get; set; }

        public int NumberOfTickets { get; set; }

        public long TicketPriceInCents { get; set; }
    }
}