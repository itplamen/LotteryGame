namespace LotteryGame.Clients.Core.Services.Models.Betting
{
    public class BettingResponse
    {
        public bool Success { get; set; }

        public string ErrorMsg { get; set; }

        public decimal TotalCost { get; set; }

        public IEnumerable<string> TicketNumbers { get; set; }

        public string DrawId { get; set; }

        public DateTime DrawDate { get; set; }
    }
}
