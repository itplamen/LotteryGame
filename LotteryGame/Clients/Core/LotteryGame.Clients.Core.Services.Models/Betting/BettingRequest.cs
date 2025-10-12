namespace LotteryGame.Clients.Core.Services.Models.Betting
{
    public class BettingRequest
    {
        public BettingRequest(int playerId, int numberOfTickets)
        {
            PlayerId = playerId;
            NumberOfTickets = numberOfTickets;
        }

        public int PlayerId { get; set; }

        public int NumberOfTickets { get; set; }
    }
}
