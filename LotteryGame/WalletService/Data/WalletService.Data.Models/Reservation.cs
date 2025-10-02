namespace WalletService.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    
    public class Reservation : BaseEntity
    {
        [Required]
        public long Amount { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsCaptured { get; set; }

        public string TicketId { get; set; }

        [Required]
        public int WalletId { get; set; }

        public Wallet Wallet { get; set; }
    }
}
