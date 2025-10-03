namespace DrawService.Core.PrizeDeterminations
{
    using Microsoft.Extensions.Configuration;
   
    using DrawService.Data.Models;

    public class SecondTierPrizeStrategy : ProportionalTierPrizeStrategy
    {
        public SecondTierPrizeStrategy(IConfiguration configuration)
        {
            RevenueShare = decimal.Parse(configuration["SecondPrizeRevenueShare"]);
            TicketShare = decimal.Parse(configuration["SecondPrizeTicketShare"]);
            Tier = PrizeTier.Second;
        }

        protected override decimal RevenueShare { get; }

        protected override decimal TicketShare { get; }
        
        protected override PrizeTier Tier { get; }
    }
}
