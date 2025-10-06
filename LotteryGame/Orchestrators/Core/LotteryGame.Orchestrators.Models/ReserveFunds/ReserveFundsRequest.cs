namespace LotteryGame.Orchestrators.Models.ReserveFunds
{
    public class ReserveFundsRequest
    {
        public int PlayerId { get; set; }

        public int NumberOfTickets { get; set; }

        public long TicketPriceInCents { get; set; }
    }
}