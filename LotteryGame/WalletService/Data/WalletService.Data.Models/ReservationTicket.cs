namespace WalletService.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ReservationTicket
    {
        [Required]
        public int ReservationId { get; set; }

        public Reservation Reservation { get; set; }

        [Required]
        public string TicketId { get; set; }
    }
}
