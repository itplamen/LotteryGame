namespace WalletService.Core.Operations
{
    using Microsoft.Extensions.Configuration;

    using LotteryGame.Common.Models.Dto;

    using WalletService.Core.Contracts;
    using WalletService.Data.Contracts;
    using WalletService.Data.Models;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;

    public class FundsOperations : IFundsOperations
    {
        private readonly int reservationExpiryMins;
        private readonly IRepository<Wallet> walletRepo;
        private readonly IRepository<Reservation> reservationRepo;
        private readonly IBalanceHistoryOperations balanceHistoryOperations;
        private readonly OperationPipeline<WalletOperationContext> walletPipeline;
        private readonly OperationPipeline<ReservationOperationContext> reservationPipeline;

        public FundsOperations(
            IRepository<Wallet> walletRepo, 
            IRepository<Reservation> reservationRepo, 
            IBalanceHistoryOperations balanceHistoryOperations, 
            OperationPipeline<WalletOperationContext> walletPipeline,
            OperationPipeline<ReservationOperationContext> reservationPipeline,
            IConfiguration config)
        {
            this.walletRepo = walletRepo;
            this.reservationRepo = reservationRepo;
            this.balanceHistoryOperations = balanceHistoryOperations;
            this.walletPipeline = walletPipeline;
            this.reservationPipeline = reservationPipeline;
            this.reservationExpiryMins = int.Parse(config["Reservation:ExpiryMins"]);
        }

        public async Task<ResponseDto> HasEnoughFunds(int playerId, long cost)
        {
            var context = new WalletOperationContext { PlayerId = playerId, Amount = cost };
            ResponseDto validation = await walletPipeline.ExecuteAsync(context);
            
            return validation;
        }

        public async Task<ResponseDto<BaseDto>> Reserve(int playerId, long amount)
        {
            var context = new WalletOperationContext()
            {
                PlayerId = playerId,
                Amount = amount
            };

            ResponseDto validation = await walletPipeline.ExecuteAsync(context);
            if (!validation.IsSuccess)
            {
                return new ResponseDto<BaseDto>(validation.ErrorMsg);
            }

            Wallet wallet = context.Wallet!;
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
                reservation.Id);

            return new ResponseDto<BaseDto>() { Data = new BaseDto(reservation.Id.ToString()) };
        }

        public async Task<ResponseDto> Capture(int reservationId)
        {
            var context = new ReservationOperationContext()
            {
                ReservationId = reservationId
            };

            ResponseDto validation = await reservationPipeline.ExecuteAsync(context);
            if (!validation.IsSuccess)
            {
                return validation;
            }    

            Wallet wallet = context.Wallet!;
            Reservation reservation = context.Reservation!;

            long oldBalance = wallet.TotalBalance;
            wallet.LockedFunds -= reservation.Amount;
            reservation.IsCaptured = true;

            await walletRepo.SaveChangesAsync();

            await balanceHistoryOperations.Record(
                wallet.Id,
                oldBalance,
                wallet.TotalBalance,
                BalanceType.Capture,
                "Funds captured",
                reservation.Id);

            return new ResponseDto();
        }

        public async Task<ResponseDto> Refund(int reservationId)
        {
            var context = new ReservationOperationContext()
            {
                ReservationId = reservationId
            };

            ResponseDto validation = await reservationPipeline.ExecuteAsync(context);
            if (!validation.IsSuccess)
            {
                return validation;
            }

            Wallet wallet = context.Wallet!;
            Reservation reservation = context.Reservation!;

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
                reservation.Id);

            return new ResponseDto();
        }
    }
}