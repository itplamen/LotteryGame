namespace LotteryGame.Clients.Core.Services.Models.Profile
{
    public class ProfileRequest
    {
        public ProfileRequest(int playerId)
        {
            PlayerId = playerId;
        }

        public int PlayerId { get; set; }
    }
}
