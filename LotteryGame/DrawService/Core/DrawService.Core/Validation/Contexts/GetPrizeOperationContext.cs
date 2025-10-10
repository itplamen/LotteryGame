namespace DrawService.Core.Validation.Contexts
{
    public class GetPrizeOperationContext : BaseDrawContext
    {
        public IEnumerable<string> PrizeIds { get; set; }
    }
}
