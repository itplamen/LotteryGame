namespace WalletService.Core.Operations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    using LotteryGame.Common.Models.Dto;

    using WalletService.Core.Contracts;
    using WalletService.Data.Contracts;
    using WalletService.Data.Models;

    public class FundsOperations : IFundsOperations
    {
        private readonly int reservationExpiryMins;
        private readonly IRepository<Wallet> walletRepo;
        private readonly IRepository<Reservation> reservationRepo;
        private readonly IBalanceHistoryOperations balanceHistoryOperations;

        public FundsOperations(
            IRepository<Wallet> walletRepo, 
            IRepository<Reservation> reservationRepo, 
            IBalanceHistoryOperations balanceHistoryOperations, 
            IConfiguration config)
        {
            this.walletRepo = walletRepo;
            this.reservationRepo = reservationRepo;
            this.balanceHistoryOperations = balanceHistoryOperations;
            this.reservationExpiryMins = int.Parse(config["Reservation:ExpiryMins"]);
        }

        public async Task<ResponseDto> HasEnoughFunds(int playerId, long costAmount)
        {
            Wallet wallet = await walletRepo.Filter()
                .FirstOrDefaultAsync(x => x.PlayerId == playerId);

            if (wallet == null)
            {
                return new ResponseDto("Wallet not found");
            }

            string errorMsg = wallet.TotalBalance >= costAmount ? null : "Insufficient funds";
            return new ResponseDto() { ErrorMsg = errorMsg };
        }

        public async Task<ResponseDto<BaseDto>> Reserve(int playerId, long amount)
        {
            Wallet wallet = await walletRepo.Filter()
                .FirstOrDefaultAsync(x => x.PlayerId == playerId);

            if (wallet == null)
            {
                return new ResponseDto<BaseDto>("Wallet not found");
            }

            if (wallet.RealMoney + wallet.BonusMoney < amount)
            {
                return new ResponseDto<BaseDto>("Insufficient funds");
            }

            long oldBalance = wallet.TotalBalance;
            long remaining = amount;

            if (wallet.RealMoney >= remaining)
            {
                wallet.RealMoney -= remaining;
            }
            else
            {
                remaining -= wallet.RealMoney;
                wallet.RealMoney = 0;
                wallet.BonusMoney -= remaining;
            }

            wallet.LockedFunds += amount;

            var reservation = new Reservation()
            {
                WalletId = wallet.Id,
                Amount = amount,
                IsCaptured = false,
                ExpiresAt = DateTime.UtcNow.AddMinutes(reservationExpiryMins)
            };

            await reservationRepo.AddAsync(reservation);
            await walletRepo.SaveChangesAsync();

            await balanceHistoryOperations.Record(
                wallet.Id,
                oldBalance,
                wallet.TotalBalance, 
                BalanceType.Reserve, 
                "Funds reserved",
                reservation.TicketId);

            return new ResponseDto<BaseDto>() { Data = new BaseDto(reservation.Id.ToString()) };
        }

        public async Task<ResponseDto> Capture(int reservationId, string ticketId)
        {
            Reservation reservation = await reservationRepo.GetByIdAsync(reservationId);
            if (reservation == null || reservation.IsCaptured)
            {
                return new ResponseDto("Invalid reservation");
            }

            Wallet wallet = await walletRepo.GetByIdAsync(reservation.WalletId);
            if (wallet == null)
            {
                 return new ResponseDto("Wallet not found");
            }

            long oldBalance = wallet.TotalBalance;
            wallet.LockedFunds -= reservation.Amount;

            reservation.IsCaptured = true;
            reservation.TicketId = ticketId;

            await walletRepo.SaveChangesAsync();

            await balanceHistoryOperations.Record(
                wallet.Id,
                oldBalance,
                wallet.TotalBalance,
                BalanceType.Capture,
                "Funds captured",
                reservation.TicketId);

            return new ResponseDto();
        }

        public async Task<ResponseDto> Refund(int reservationId)
        {
            Reservation reservation = await reservationRepo.GetByIdAsync(reservationId);
            if (reservation == null || reservation.IsCaptured)
            {
                return new ResponseDto("Invalid reservation");
            }

            Wallet wallet = await walletRepo.GetByIdAsync(reservation.WalletId);
            if (wallet == null)
            {
                return new ResponseDto("Wallet not found");
            }

            if (wallet.LockedFunds < reservation.Amount)
            {
                return new ResponseDto("Insufficient locked funds to process refund");
            }

            long oldBalance = wallet.TotalBalance;
            wallet.LockedFunds -= reservation.Amount;
            wallet.RealMoney += reservation.Amount;

            reservationRepo.Delete(reservation);
            await reservationRepo.SaveChangesAsync();

            await balanceHistoryOperations.Record(
                wallet.Id,
                oldBalance,
                wallet.TotalBalance,
                BalanceType.Refund,
                "Funds refunded",
                reservation.TicketId);

            return new ResponseDto();
        }
    }
}