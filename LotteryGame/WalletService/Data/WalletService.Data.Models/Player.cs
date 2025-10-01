namespace WalletService.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Player : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public Wallet Wallet { get; set; } = new Wallet();
    }
}
