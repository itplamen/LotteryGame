namespace WagerService.Data.Models
{
    public class Ticket : BaseEntity
    {
        public string TicketNumber { get; set; }

        public int PlayerId { get; set; }

        public string DrawId { get; set; }

        public int ReservationId { get; set; }

        public TicketStatus Status { get; set; }
    }
}
