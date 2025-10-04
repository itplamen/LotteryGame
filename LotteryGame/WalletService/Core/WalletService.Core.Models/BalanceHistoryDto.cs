namespace WalletService.Core.Models
{
    using WalletService.Data.Models;

    public class BalanceHistoryDto
    {
        public long Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public long OldBalance { get; set; }

        public long NewBalance { get; set; }
 
        public BalanceType Type { get; set; }
 
        public string Reason { get; set; }

        public bool IsConfirmed { get; set; }
    }
}
