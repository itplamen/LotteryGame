namespace LotteryGame.Clients.Core.Services.Models.History
{
    public class HistoryRequest
    {
        public HistoryRequest(string drawId)
        {
            DrawId = drawId;
        }

        public string DrawId { get; set; }
    }
}
