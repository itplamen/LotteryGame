namespace WalletService.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class BalanceHistory : BaseEntity
    {
        [Required]  
        public long OldBalanceInCents { get; set; }

        [Required]
        public long NewBalanceInCents { get; set; }

        [Required]
        public BalanceType Type { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public int WalletId { get; set; }

        public Wallet Wallet { get; set; }

        [Required]
        public int ReservationId { get; set; }

        public Reservation Reservation { get; set; }
    }
}
