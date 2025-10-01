namespace WalletService.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Reservation : BaseEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsCaptured { get; set; }

        [Required]
        public string TicketId { get; set; }

        [Required]
        public int WalletId { get; set; }

        public Wallet Wallet { get; set; }
    }
}
