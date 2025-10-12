namespace LotteryGame.Clients.Core.Services.Models
{
    public class PurchaseTicketsResponse
    {
        public bool Success { get; set; }

        public string ErrorMsg { get; set; }

        public decimal TotalCost { get; set; }

        public IEnumerable<string> TicketNumbers { get; set; }

        public string DrawId { get; set; }

        public DateTime DrawDate { get; set; }
    }
}
