namespace DrawService.Data
{
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;

    public class DrawConflictResolver : IConflictResolver<Draw>
    {
        public Draw Resolve(Draw latest, Draw incoming)
        {
            foreach (var playerTicketInfo in incoming.PlayerTickets)
            {
                PlayerTicketInfo existing = latest.PlayerTickets.FirstOrDefault(x => x.PlayerId == playerTicketInfo.PlayerId);
                
                if (existing != null)
                {
                    existing.Tickets.AddRange(playerTicketInfo.Tickets.Except(existing.Tickets));
                }
                else
                {
                    latest.PlayerTickets.Add(playerTicketInfo);
                }
            }

            return latest;
        }
    }
}
