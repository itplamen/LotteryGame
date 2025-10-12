namespace DrawService.Core.Models
{
    public class HistoryDto
    {
        public DateTime DrawDate { get; set; }

        public string DrawStatus { get; set; }

        public int Participants { get; set; }

        public long HouseProfitInCents { get; set; }

        public IEnumerable<PrizeDto> Prizes { get; set; }
    }
}
