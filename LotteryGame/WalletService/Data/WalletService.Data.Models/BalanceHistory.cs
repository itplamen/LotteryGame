namespace WalletService.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class BalanceHistory : BaseEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OldBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NewBalance { get; set; }

        [Required]
        public string ReferenceId { get; set; }

        [Required]
        public BalanceType Type { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public int WalletId { get; set; }

        public Wallet Wallet { get; set; }
    }
}
