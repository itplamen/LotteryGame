namespace LotteryGame.Orchestrators.Models.Base
{
    public class OrchestratorRequest<TData>
    {
        public OrchestratorRequest(TData payload)
        {
            Payload = payload;
        }

        public string CorrelationId { get; } = Guid.NewGuid().ToString();

        public DateTime Timestamp { get; } = DateTime.UtcNow;
        
        public TData Payload { get; set; }
    }
}
