namespace LotteryGame.Orchestrators.Models.PlayerProfile
{
    public class PlayerProfileResponse
    {
        public long RealBalanceInCents { get; set; }

        public long BonusBalanceInCents { get; set; }

        public DrawOptions DrawOptions { get; set; }
    }
}
