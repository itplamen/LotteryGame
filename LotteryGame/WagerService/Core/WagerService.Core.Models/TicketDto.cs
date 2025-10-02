namespace WagerService.Core.Models
{
    using WagerService.Data.Models;

    public class TicketDto : BaseDto
    {
        public string TicketNumber { get; set; }

        public TicketStatus Status { get; set; }
    }
}
