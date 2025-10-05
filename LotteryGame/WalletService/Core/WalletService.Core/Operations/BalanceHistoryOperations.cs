namespace WalletService.Core.Operations
{
    using AutoMapper;

    using Microsoft.EntityFrameworkCore;
    
    using WalletService.Core.Contracts;
    using WalletService.Core.Models;
    using WalletService.Data.Contracts;
    using WalletService.Data.Models;

    public class BalanceHistoryOperations : IBalanceHistoryOperations
    {
        private readonly IMapper mapper;
        private readonly IRepository<BalanceHistory> repository;

        public BalanceHistoryOperations(IMapper mapper, IRepository<BalanceHistory> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public async Task Record(
            int walletId, 
            long oldBalance, 
            long newBalance, 
            BalanceType balanceType, 
            string reason,
            int reservationId)
        {
            var balanceHistory = new BalanceHistory()
            {
                WalletId = walletId,
                OldBalance = oldBalance,
                NewBalance = newBalance,
                Type = balanceType,
                Reason = reason,
                ReservationId = reservationId
            };

            await repository.AddAsync(balanceHistory);
            await repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<BalanceHistoryDto>> Get(int playerId)
        {
            IEnumerable<BalanceHistory> history = await repository.Filter()
                .Include(x => x.Wallet)
                .Include(x => x.Reservation)
                .Where(x => x.Wallet.PlayerId == playerId)
                .OrderBy(x => x.CreatedOn)
                .ToListAsync();

            return mapper.Map<IEnumerable<BalanceHistoryDto>>(history);
        }
    }
}
