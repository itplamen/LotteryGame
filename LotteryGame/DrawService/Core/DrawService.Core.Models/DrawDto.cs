namespace DrawService.Core.Models
{
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.Dto;

    public class DrawDto : BaseDto
    {
        public DateTime DrawDate { get; set; }

        public DrawStatus Status { get; set; }

        public long TicketPriceInCents { get; set; }

        public int MinTicketsPerPlayer { get; set; }

        public int MaxTicketsPerPlayer { get; set; }
    }
}
