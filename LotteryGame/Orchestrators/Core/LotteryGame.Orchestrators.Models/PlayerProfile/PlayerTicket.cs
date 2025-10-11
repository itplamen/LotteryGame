namespace LotteryGame.Orchestrators.Models.PlayerProfile
{
    public class PlayerTicket
    {
        public string Number { get; set; }

        public string Status { get; set; }

        public PlayerPrize Prize { get; set; }
    }
}
