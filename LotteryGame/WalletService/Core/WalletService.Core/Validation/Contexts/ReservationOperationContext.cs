namespace WalletService.Core.Validation.Contexts
{
    using WalletService.Data.Models;

    public class ReservationOperationContext
    {
        public int ReservationId { get; set; }

        public Reservation Reservation { get; set; }
        
        public Wallet Wallet { get; set; }
        
        public long Amount { get; set; }
    }
}
