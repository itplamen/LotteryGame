namespace LotteryGame.Orchestrators.Models.PlayerProfile
{
    public class PlayerProfileResponse
    {
        public long RealBalance { get; set; }

        public long BonusBalance { get; set; }

        public DrawOptions DrawOptions { get; set; }
    }
}
