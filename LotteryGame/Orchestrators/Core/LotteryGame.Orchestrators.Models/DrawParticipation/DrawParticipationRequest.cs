namespace LotteryGame.Orchestrators.Models.DrawParticipation
{
    public class DrawParticipationRequest
    {
        public int PlayerId { get; set; }

        public string DrawId { get; set; }

        public IEnumerable<string> TicketIds { get; set; }
    }
}