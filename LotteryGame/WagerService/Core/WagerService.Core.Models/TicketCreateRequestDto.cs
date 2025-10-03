namespace WagerService.Core.Models
{
    public class TicketCreateRequestDto
    {
        public int PlayerId { get; set; }

        public string DrawId { get; set; }

        public int ReservationId { get; set; }
    }
}
