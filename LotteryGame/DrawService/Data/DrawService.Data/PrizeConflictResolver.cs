namespace DrawService.Data
{
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;

    public class PrizeConflictResolver : IConflictResolver<Prize>
    {
        public Prize Resolve(Prize latest, Prize incoming)
        {
            latest.AmountInCents = Math.Max(latest.AmountInCents, incoming.AmountInCents);
            latest.Tier = incoming.Tier;
            latest.TicketId = incoming.TicketId;
            latest.DrawId = incoming.DrawId;
            latest.ModifiedOn = DateTime.UtcNow;

            return latest;
        }
    }
}
