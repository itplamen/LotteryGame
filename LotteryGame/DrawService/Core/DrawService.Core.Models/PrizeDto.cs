namespace DrawService.Core.Models
{
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.Dto;

    public class PrizeDto : BaseDto
    {
        public string TicketId { get; set; }

        public long Amount { get; set; }

        public PrizeTier Tier { get; set; }

        public string DrawId { get; set; }
    }
}
