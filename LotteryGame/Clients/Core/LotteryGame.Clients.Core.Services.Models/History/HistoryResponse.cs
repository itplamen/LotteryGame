namespace LotteryGame.Clients.Core.Services.Models.History
{
    public class HistoryResponse
    {
        public bool Success { get; set; }

        public string ErrorMsg { get; set; }

        public DateTime DrawDate { get; set; }

        public string DrawStatus { get; set; }

        public int Participants { get; set; }

        public decimal HouseProfit { get; set; }

        public IEnumerable<PrizeHistory> Prizes { get; set; }
    }
}
