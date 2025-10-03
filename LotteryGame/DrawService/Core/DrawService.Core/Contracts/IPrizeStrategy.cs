namespace DrawService.Core.Contracts
{
    using DrawService.Data.Models;

    public interface IPrizeStrategy
    {
        IEnumerable<Prize> Calculate(Draw draw, List<string> remainingTicketIds, long totalRevenue);
    }
}
