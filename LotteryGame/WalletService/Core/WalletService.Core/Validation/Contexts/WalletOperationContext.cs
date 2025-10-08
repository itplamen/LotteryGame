namespace WalletService.Core.Validation.Contexts
{
    using WalletService.Data.Models;

    public class WalletOperationContext
    {
        public int PlayerId { get; set; }

        public long Amount { get; set; }
        
        public Wallet Wallet { get; set; }
    }
}
