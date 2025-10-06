namespace LotteryGame.Orchestrators.Models.Base
{
    public class OrchestratorResponse<TData>
    {
        public bool Success { get; set; }

        public string ErrorMsg { get; set; }

        public TData Data { get; set; }
    }
}
