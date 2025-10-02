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
        public long RealMoney { get; set; }

        [Required]
        public long BonusMoney { get; set; }

        [Required]
        public long LockedFunds { get; set; }

        [Required]
        public int LoyaltyPoints { get; set; }
        
        [Required]
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public ICollection<BalanceHistory> BalanceHistories { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

        [NotMapped]
        public long TotalBalance => RealMoney + BonusMoney + LockedFunds;
    }
}
