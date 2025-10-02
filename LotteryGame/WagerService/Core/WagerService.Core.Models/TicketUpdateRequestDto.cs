namespace WagerService.Core.Models
{
    using WagerService.Data.Models;

    public class TicketUpdateRequestDto
    {
        public string TicketId { get; set; }

        public TicketStatus Status { get; set; }
    }
}
