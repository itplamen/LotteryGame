namespace LotteryGame.Clients.Core.Services.Models.Profile
{
    public class ProfileResponse
    {
        public bool Success { get; set; }

        public string ErrorMsg { get; set; }

        public decimal RealBalance { get; set; }

        public decimal BonusBalance { get; set; }

        public DrawOptions DrawOptions { get; set; }
    }
}
