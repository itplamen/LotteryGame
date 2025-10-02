namespace WalletService.Core.Operations
{
    using WalletService.Core.Contracts;
    using WalletService.Data.Contracts;
    using WalletService.Data.Models;

    public class BalanceHistoryOperations : IBalanceHistoryOperations
    {
        private readonly IRepository<BalanceHistory> repository;

        public BalanceHistoryOperations(IRepository<BalanceHistory> repository)
        {
            this.repository = repository;
        }

        public async Task Record(int walletId, long oldBalance, long newBalance, BalanceType balanceType, string reason, string referenceId = null)
        {
            var balanceHistory = new BalanceHistory()
            {
                WalletId = walletId,
                OldBalance = oldBalance,
                NewBalance = newBalance,
                Type = balanceType,
                Reason = reason,
                ReferenceId = referenceId
            };

            await repository.AddAsync(balanceHistory);
            await repository.SaveChangesAsync();
        }
    }
}
