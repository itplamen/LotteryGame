namespace WagerService.Core.Models
{
    using WagerService.Data.Models;

    public class TicketUpdateRequestDto
    {
        public IEnumerable<string> TicketIds { get; set; }

        public TicketStatus Status { get; set; }
    }
}
