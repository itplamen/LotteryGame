namespace DrawService.Core.PrizeDeterminations
{
    using Microsoft.Extensions.Configuration;
   
    using DrawService.Data.Models;

    public class SecondTierPrizeStrategy : ProportionalTierPrizeStrategy
    {
        public SecondTierPrizeStrategy(IConfiguration configuration)
        {
            RevenueShare = decimal.Parse(configuration["Prize:SecondPrizeRevenueShare"]);
            TicketShare = decimal.Parse(configuration["Prize:SecondPrizeTicketShare"]);
            Tier = PrizeTier.SecondTier;
        }

        protected override decimal RevenueShare { get; }

        protected override decimal TicketShare { get; }
        
        protected override PrizeTier Tier { get; }
    }
}
