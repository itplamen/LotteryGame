namespace WalletService.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Wallet : BaseEntity
    {
        public Wallet()
        {
            BalanceHistories = new HashSet<BalanceHistory>();
            Reservations = new HashSet<Reservation>();
        }

        [Required]
        public long RealMoneyInCents { get; set; }

        [Required]
        public long BonusMoneyInCents { get; set; }

        [Required]
        public long LockedFundsInCents { get; set; }

        [Required]
        public int LoyaltyPoints { get; set; }
        
        [Required]
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public ICollection<BalanceHistory> BalanceHistories { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

        [NotMapped]
        public long TotalBalanceInCents => RealMoneyInCents + BonusMoneyInCents + LockedFundsInCents;
    }
}
