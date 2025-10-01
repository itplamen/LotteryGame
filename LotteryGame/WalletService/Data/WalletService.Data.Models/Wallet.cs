namespace WalletService.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Wallet : BaseEntity
    {
        public Wallet()
        {
            BalanceHistories = new HashSet<BalanceHistory>();
        }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RealMoney { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BonusMoney { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LockedFunds { get; set; }

        [Required]
        public int LoyaltyPoints { get; set; }
        
        [Required]
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public ICollection<BalanceHistory> BalanceHistories { get; set; }
    }
}
