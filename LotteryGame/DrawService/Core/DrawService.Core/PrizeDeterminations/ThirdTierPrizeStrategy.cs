namespace DrawService.Core.PrizeDeterminations
{
    using Microsoft.Extensions.Configuration;
    
    using DrawService.Data.Models;

    public class ThirdTierPrizeStrategy : ProportionalTierPrizeStrategy
    {
        public ThirdTierPrizeStrategy(IConfiguration configuration)
        {
            RevenueShare = decimal.Parse(configuration["ThirdPrizeRevenueShare"]);
            TicketShare = decimal.Parse(configuration["ThirdPrizeTicketShare"]);
            Tier = PrizeTier.Third;
        }

        protected override decimal RevenueShare { get; }

        protected override decimal TicketShare { get; }

        protected override PrizeTier Tier { get; }
    }
}
