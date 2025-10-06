namespace LotteryGame.Orchestrators.Models.PurchaseTickets
{
    public class PurchaseTicketsRequest
    {
        public int PlayerId { get; set; }

        public string DrawId { get; set; }

        public int ReservationId { get; set; }

        public int NumberOfTickets { get; set; }
    }
}
