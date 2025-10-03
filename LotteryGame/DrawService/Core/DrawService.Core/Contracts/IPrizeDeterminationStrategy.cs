namespace DrawService.Core.Contracts
{
    using DrawService.Data.Models;

    public interface IPrizeDeterminationStrategy
    {
        IEnumerable<Prize> DeterminePrizes(Draw draw);
    }
}
