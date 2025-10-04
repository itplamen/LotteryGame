namespace WalletService.Core.Contracts
{
    using WalletService.Core.Models;
    using WalletService.Data.Models;

    public interface IBalanceHistoryOperations
    {
        Task Record(int walletId, long oldBalance, long newBalance, BalanceType balanceType, string reason);

        Task<IEnumerable<BalanceHistoryDto>> Get(int playerId);
    }
}
