namespace WalletService.Core.Operations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    using WalletService.Core.Contracts;
    using WalletService.Core.Models;
    using WalletService.Data.Contracts;
    using WalletService.Data.Models;

    public class FundsOperations : IFundsOperations
    {
        private readonly int reservationExpiryMins;
        private readonly IRepository<Wallet> walletRepo;
        private readonly IRepository<Reservation> reservationRepo;

        public FundsOperations(IRepository<Wallet> walletRepo, IRepository<Reservation> reservationRepo, IConfiguration config)
        {
            this.walletRepo = walletRepo;
            this.reservationRepo = reservationRepo;
            this.reservationExpiryMins = int.Parse(config["Reservation:ExpiryMins"]);
        }

        public async Task<ResponseDto<BaseDto>> Reserve(int playerId, decimal amount, string ticketId)
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

            decimal remaining = amount;

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
                TicketId = ticketId,
                IsCaptured = false,
                ExpiresAt = DateTime.UtcNow.AddMinutes(reservationExpiryMins)
            };

            await reservationRepo.AddAsync(reservation);
            await walletRepo.SaveChangesAsync();

            return new ResponseDto<BaseDto>() { Data = new BaseDto(reservation.Id) };
        }

        public async Task<ResponseDto> Capture(int reservationId)
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

            wallet.LockedFunds -= reservation.Amount;
            reservation.IsCaptured = true;

            await walletRepo.SaveChangesAsync();

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

            wallet.LockedFunds -= reservation.Amount;
            wallet.RealMoney += reservation.Amount;

            reservationRepo.Delete(reservation);
            await reservationRepo.SaveChangesAsync();

            return new ResponseDto();
        }
    }
}