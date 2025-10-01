namespace WalletService.Core.Contracts
{
    using WalletService.Data.Models;

    public interface IBalanceHistoryOperations
    {
        Task Record(int walletId, decimal oldBalance, decimal newBalance, BalanceType balanceType, string reason, string referenceId = null);
    }
}
