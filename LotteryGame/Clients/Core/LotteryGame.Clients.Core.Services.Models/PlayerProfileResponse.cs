namespace LotteryGame.Clients.Core.Services.Models
{
    public class PlayerProfileResponse
    {
        public bool Success { get; set; }

        public string ErrorMsg { get; set; }

        public decimal RealBalance { get; set; }

        public decimal BonusBalance { get; set; }

        public DrawOptions DrawOptions { get; set; }
    }
}
