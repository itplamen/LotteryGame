namespace DrawService.Core.PrizeDeterminations
{
    using System.Security.Cryptography;

    using Microsoft.Extensions.Configuration;

    using DrawService.Core.Contracts;
    using DrawService.Data.Models;

    public class GrandPrizeStrategy : IPrizeStrategy
    {
        private readonly decimal revenueShare;

        public GrandPrizeStrategy(IConfiguration configuration)
        {
            revenueShare = decimal.Parse(configuration["Prize:GrandPrizeRevenueShare"]);
        }

        public IEnumerable<Prize> Calculate(Draw draw, List<string> remainingTicketIds, long totalRevenue)
        {
            if (remainingTicketIds == null || !remainingTicketIds.Any())
            {
                return new List<Prize>();
            }

            int winnerIndex = RandomNumberGenerator.GetInt32(remainingTicketIds.Count);
            string winningTicketId = remainingTicketIds[winnerIndex];

            var grandPrize = new Prize()
            {
                DrawId = draw.Id,
                TicketId = winningTicketId,
                AmountInCents = (long)(totalRevenue * revenueShare),
                Tier = PrizeTier.GrandPrize
            };

            return new List<Prize> { grandPrize };
        }
    }
}
