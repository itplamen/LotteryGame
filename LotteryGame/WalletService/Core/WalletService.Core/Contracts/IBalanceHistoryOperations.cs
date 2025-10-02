namespace WalletService.Core.Contracts
{
    using WalletService.Data.Models;

    public interface IBalanceHistoryOperations
    {
        Task Record(int walletId, long oldBalance, long newBalance, BalanceType balanceType, string reason, string referenceId = null);
    }
}
