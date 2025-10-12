namespace LotteryGame.Orchestrators.Models.DrawHistory
{
    public class DrawHistoryResponse
    {
        public DateTime DrawDate { get; set; }

        public string DrawStatus { get; set; }

        public int Participants { get; set; }

        public long HouseProfitInCents { get; set; }

        public IEnumerable<PrizeHistory> Prizes { get; set; }
    }
}
