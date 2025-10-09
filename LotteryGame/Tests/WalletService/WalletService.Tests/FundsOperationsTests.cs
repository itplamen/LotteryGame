namespace WalletService.Tests
{
    using FluentAssertions;
    
    using Microsoft.Extensions.Configuration;

    using Moq;

    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Contracts;
    using WalletService.Core.Operations;
    using WalletService.Core.Validation.Contexts;
    using WalletService.Core.Validation.Policies;
    using WalletService.Data.Models;
    using WalletService.Tests.Utils.Database;
    using WalletService.Tests.Utils.Stubs;

    [TestFixture]
    public class FundsOperationsTests
    {
        private DbRepositoryMock<Wallet> walletRepoMock;
        private DbRepositoryMock<Reservation> reservationRepoMock;
        private Mock<IBalanceHistoryOperations> balanceHistoryMock;
        private IConfiguration configs;
        private FundsOperations fundsOperations;

        [SetUp]
        public void Setup()
        {
            walletRepoMock = new DbRepositoryMock<Wallet>(WalletStubs.GetWallets());
            reservationRepoMock = new DbRepositoryMock< Reservation>();
            balanceHistoryMock = new Mock<IBalanceHistoryOperations>();

            var configDict = new Dictionary<string, string>
            {
                ["Reservation:ExpiryMins"] = "15"
            };
            configs = new ConfigurationBuilder().AddInMemoryCollection(configDict).Build();

            var walletPolicies = new List<IOperationPolicy<WalletOperationContext>>()
            {
                new ValidatePlayerIdPolicy(),
                new ValidateWalletExistsPolicy(walletRepoMock.Mock),
                new ValidateSufficientFundsPolicy()
            };

            var walletPipeline = new OperationPipeline<WalletOperationContext>(walletPolicies);

            var reservationPolicies = new List<IOperationPolicy<ReservationOperationContext>>()
            {
                new ValidateReservationExistsPolicy(reservationRepoMock.Mock),
                new ValidateWalletForReservationPolicy(walletRepoMock.Mock),
                new ValidateReservationNotCapturedPolicy(),
                new ValidateReservationNotExpiredPolicy(),
                new ValidateReservationNotRefundedPolicy(),
                new ValidateRefundableFundsPolicy()
            };

            var reservationPipeline = new OperationPipeline<ReservationOperationContext>(reservationPolicies);

            fundsOperations = new FundsOperations(
                walletRepoMock.Mock,
                reservationRepoMock.Mock,
                balanceHistoryMock.Object,
                walletPipeline,
                reservationPipeline,
                configs);
        }

        [Test]
        public async Task HasEnoughFunds_ShouldReturnError_WhenPlayerIdIsInvalid()
        {
            int invalidId = -111;
            var result = await fundsOperations.HasEnoughFunds(invalidId, 1);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Invalid player Id");
        }

        [Test]
        public async Task HasEnoughFunds_ShouldReturnError_WhenWalletNotFound()
        {
            int invalidId = PlayersStub.GetPlayers().Max(x => x.Id) + 1;
            var result = await fundsOperations.HasEnoughFunds(invalidId, 1);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Wallet not found");
        }

        [Test]
        public async Task HasEnoughFunds_ShouldReturnError_WhenInsufficientFunds()
        {
            Wallet wallet = WalletStubs.GetWallets().First();
            var result = await fundsOperations.HasEnoughFunds(wallet.Id, wallet.TotalBalance + 1);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Insufficient funds");
        }

        [Test]
        public async Task HasEnoughFunds_ShouldReturnSuccess_WhenSufficientFunds()
        {
            Wallet wallet = WalletStubs.GetWallets().First();
            var result = await fundsOperations.HasEnoughFunds(wallet.Id, wallet.TotalBalance - 1);

            result.IsSuccess.Should().BeTrue();
            result.ErrorMsg.Should().Be(string.Empty);
        }

        [Test]
        public async Task Reserve_ShouldReturnError_WhenPlayerIdIsInvalid()
        {
            int invalidId = -111;
            var result = await fundsOperations.Reserve(invalidId, 100);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Invalid player Id");
            result.Data.Should().BeNull();
        }

        [Test]
        public async Task Reserve_ShouldReturnError_WhenWalletNotFound()
        {
            int invalidId = PlayersStub.GetPlayers().Max(x => x.Id) + 1;
            var result = await fundsOperations.Reserve(invalidId, 1);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Wallet not found");
            result.Data.Should().BeNull();
        }

        [Test]
        public async Task Reserve_ShouldReturnError_WhenInsufficientFunds()
        {
            Wallet wallet = WalletStubs.GetWallets().First();
            var result = await fundsOperations.Reserve(wallet.Id, wallet.TotalBalance + 1);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Insufficient funds");
            result.Data.Should().BeNull();
        }

        [Test]
        public async Task Reserve_ShouldDecreaseOnlyRealBalance_AndCreateReservation()
        {
            var wallet = walletRepoMock.Data.First();
            long initialRealMoney = wallet.RealMoney;
            long initialBonusMoney = wallet.BonusMoney;
            long reserveAmount = wallet.RealMoney - 1;

            var result = await fundsOperations.Reserve(wallet.PlayerId, reserveAmount);

            result.IsSuccess.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be("1");

            wallet.RealMoney.Should().Be(initialRealMoney - reserveAmount);
            wallet.BonusMoney.Should().Be(initialBonusMoney);
            wallet.LockedFunds.Should().Be(reserveAmount);

            var reservation = reservationRepoMock.Data.FirstOrDefault(x => x.Id == long.Parse(result.Data.Id));
            reservation.Should().NotBeNull();
            reservation.Amount.Should().Be(reserveAmount);
            reservation.WalletId.Should().Be(wallet.Id);
            reservation.IsCaptured.Should().BeFalse();
        }

        [Test]
        public async Task Reserve_ShouldDecreaseBothRealAndBonusBalances_AndCreateReservation()
        {
            var wallet = walletRepoMock.Data.First();
            long initialRealMoney = wallet.RealMoney;
            long initialBonusMoney = wallet.BonusMoney;
            long reserveAmount = wallet.RealMoney + 1;

            var result = await fundsOperations.Reserve(wallet.PlayerId, reserveAmount);

            result.IsSuccess.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be("1");

            wallet.RealMoney.Should().Be(0);
            wallet.BonusMoney.Should().Be(initialBonusMoney - 1);
            wallet.LockedFunds.Should().Be(reserveAmount);

            var reservation = reservationRepoMock.Data.FirstOrDefault(x => x.Id == long.Parse(result.Data.Id));
            reservation.Should().NotBeNull();
            reservation.Amount.Should().Be(reserveAmount);
            reservation.WalletId.Should().Be(wallet.Id);
            reservation.IsCaptured.Should().BeFalse();
        }

        [Test]
        public async Task Capture_ShouldReturnError_WhenInvalidReservationId()
        {
            var result = await fundsOperations.Capture(-1);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Reservation not found");
        }

        [Test]
        public async Task Capture_ShouldReturnError_WhenReservationtNotFound()
        {
            int reservationId = (reservationRepoMock.Data?.LastOrDefault()?.Id ?? 0) + 1;
            var result = await fundsOperations.Capture(reservationId);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Reservation not found");
        }

        [Test]
        public async Task Capture_ShouldSuccessfullyCaptureReservation_AndDeductLockedWalletFunds()
        {
            var wallet = walletRepoMock.Data.First();
            long initialRealMoney = wallet.RealMoney;
            long initialBonusMoney = wallet.BonusMoney;
            long reserveAmount = 1;

            var result = await fundsOperations.Reserve(wallet.PlayerId, reserveAmount);
            result.IsSuccess.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be("1");

            wallet.RealMoney.Should().Be(initialRealMoney - reserveAmount);
            wallet.BonusMoney.Should().Be(initialBonusMoney);
            wallet.LockedFunds.Should().Be(reserveAmount);

            var resultCapture = await fundsOperations.Capture(int.Parse(result.Data.Id));
            resultCapture.IsSuccess.Should().BeTrue();
            resultCapture.ErrorMsg.Should().BeEmpty();

            wallet.RealMoney.Should().Be(initialRealMoney - reserveAmount);
            wallet.BonusMoney.Should().Be(initialBonusMoney);
            wallet.LockedFunds.Should().Be(0);

            var reservation = reservationRepoMock.Data.FirstOrDefault(x => x.Id == long.Parse(result.Data.Id));
            reservation.Should().NotBeNull();
            reservation.Amount.Should().Be(reserveAmount);
            reservation.WalletId.Should().Be(wallet.Id);
            reservation.IsCaptured.Should().BeTrue();
        }

        [Test]
        public async Task Capture_ShouldReturnError_WhenReservationIsAlreadyCaptured()
        {
            var wallet = walletRepoMock.Data.First();
            long initialRealMoney = wallet.RealMoney;
            long initialBonusMoney = wallet.BonusMoney;
            long reserveAmount = 1;

            var result = await fundsOperations.Reserve(wallet.PlayerId, reserveAmount);
            result.IsSuccess.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be("1");

            wallet.RealMoney.Should().Be(initialRealMoney - reserveAmount);
            wallet.BonusMoney.Should().Be(initialBonusMoney);
            wallet.LockedFunds.Should().Be(reserveAmount);

            var resultCapture = await fundsOperations.Capture(int.Parse(result.Data.Id));
            resultCapture.IsSuccess.Should().BeTrue();
            resultCapture.ErrorMsg.Should().BeEmpty();

            wallet.RealMoney.Should().Be(initialRealMoney - reserveAmount);
            wallet.BonusMoney.Should().Be(initialBonusMoney);
            wallet.LockedFunds.Should().Be(0);

            var reservation = reservationRepoMock.Data.FirstOrDefault(x => x.Id == long.Parse(result.Data.Id));
            reservation.Should().NotBeNull();
            reservation.Amount.Should().Be(reserveAmount);
            reservation.WalletId.Should().Be(wallet.Id);
            reservation.IsCaptured.Should().BeTrue();

            var resultNewCapture = await fundsOperations.Capture(int.Parse(result.Data.Id));
            resultNewCapture.IsSuccess.Should().BeFalse();
            resultNewCapture.ErrorMsg.Should().Be("Reservation already captured. Contact custommer support.");
        }

        [Test]
        public async Task Capture_ShouldReturnError_WhenReservationExpired()
        {
            var wallet = walletRepoMock.Data.First();
            var reserveResult = await fundsOperations.Reserve(wallet.PlayerId, 10);
            reserveResult.IsSuccess.Should().BeTrue();

            var reservation = reservationRepoMock.Data.First(x => x.Id == long.Parse(reserveResult.Data.Id));
            reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(-10);

            var captureResult = await fundsOperations.Capture(reservation.Id);

            captureResult.IsSuccess.Should().BeFalse();
            captureResult.ErrorMsg.Should().Be("Reservation expired");
        }

        [Test]
        public async Task Capture_ShouldReturnError_WhenReservationRefunded()
        {
            var wallet = walletRepoMock.Data.First();
            var reserveResult = await fundsOperations.Reserve(wallet.PlayerId, 10);
            var reservation = reservationRepoMock.Data.First(x => x.Id == long.Parse(reserveResult.Data.Id));

            await fundsOperations.Refund(reservation.Id);
            var resultCapture = await fundsOperations.Capture(reservation.Id);

            resultCapture.IsSuccess.Should().BeFalse();
            resultCapture.ErrorMsg.Should().Be("Reservation already refunded");
        }

        [Test]
        public async Task Refund_ShouldReturnError_WhenInvalidReservation()
        {
            int invalidId = -111;
            var result = await fundsOperations.Refund(invalidId);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Reservation not found");
        }

        [Test]
        public async Task Refund_ShouldReturnError_WhenReservationNotFound()
        {
            int reservationId = (reservationRepoMock.Data?.LastOrDefault()?.Id ?? 0) + 1;
            var result = await fundsOperations.Refund(reservationId);

            result.IsSuccess.Should().BeFalse();
            result.ErrorMsg.Should().Be("Reservation not found");
        }

        [Test]
        public async Task Refund_ShouldReturnError_WhenReservationAlreadyCaptured()
        {
            var wallet = walletRepoMock.Data.First();
            long initialRealMoney = wallet.RealMoney;
            long initialBonusMoney = wallet.BonusMoney;
            long reserveAmount = 1;

            var result = await fundsOperations.Reserve(wallet.PlayerId, reserveAmount);
            result.IsSuccess.Should().BeTrue();
            result.ErrorMsg.Should().BeEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be("1");

            wallet.RealMoney.Should().Be(initialRealMoney - reserveAmount);
            wallet.BonusMoney.Should().Be(initialBonusMoney);
            wallet.LockedFunds.Should().Be(reserveAmount);

            var resultCapture = await fundsOperations.Capture(int.Parse(result.Data.Id));
            resultCapture.IsSuccess.Should().BeTrue();
            resultCapture.ErrorMsg.Should().BeEmpty();

            wallet.RealMoney.Should().Be(initialRealMoney - reserveAmount);
            wallet.BonusMoney.Should().Be(initialBonusMoney);
            wallet.LockedFunds.Should().Be(0);

            var reservation = reservationRepoMock.Data.FirstOrDefault(x => x.Id == long.Parse(result.Data.Id));
            reservation.Should().NotBeNull();
            reservation.Amount.Should().Be(reserveAmount);
            reservation.WalletId.Should().Be(wallet.Id);
            reservation.IsCaptured.Should().BeTrue();

            var refundResult = await fundsOperations.Refund(reservation.Id);
            refundResult.IsSuccess.Should().BeFalse();
            refundResult.ErrorMsg.Should().Be("Reservation already captured. Contact custommer support.");
        }

        [Test]
        public async Task Refund_ShouldReturnError_WhenLockedFundsInsufficient()
        {
            var wallet = walletRepoMock.Data.First();
            long initialRealMoney = wallet.RealMoney;
            long reserveAmount = 1;

            var reserveResult = await fundsOperations.Reserve(wallet.PlayerId, reserveAmount);
            reserveResult.IsSuccess.Should().BeTrue();
            reserveResult.ErrorMsg.Should().BeEmpty();

            var reservation = reservationRepoMock.Data.FirstOrDefault(x => x.Id == long.Parse(reserveResult.Data.Id));
            reservation.Should().NotBeNull();
            reservation.Amount.Should().Be(reserveAmount);
            reservation.WalletId.Should().Be(wallet.Id);
            reservation.IsCaptured.Should().BeFalse();

            long realMoneyAfterReservation = wallet.RealMoney;

            wallet.LockedFunds = reserveAmount - 10;
            var refundResult = await fundsOperations.Refund(reservation.Id);
            
            refundResult.IsSuccess.Should().BeFalse();
            refundResult.ErrorMsg.Should().Be("Insufficient locked funds to process refund");
            reservationRepoMock.Data.Should().Contain(reservation);

            initialRealMoney.Should().BeGreaterThan(realMoneyAfterReservation);
            realMoneyAfterReservation.Should().Be(wallet.RealMoney);
            initialRealMoney.Should().Be(wallet.RealMoney + reserveAmount);
        }

        [Test]
        public async Task Refund_ShouldSuccessfullyRefundPlayer_AndReturnFundsToRealMoney()
        {
            var wallet = walletRepoMock.Data.First();
            long initialRealMoney = wallet.RealMoney;
            long initialLockedMoney = wallet.LockedFunds;
            long reserveAmount = 1;

            var reserveResult = await fundsOperations.Reserve(wallet.PlayerId, reserveAmount);
            reserveResult.IsSuccess.Should().BeTrue();
            reserveResult.ErrorMsg.Should().BeEmpty();

            long realMoneyAfterReservation = wallet.RealMoney;
            long lockedMoneyAfterReservation = wallet.LockedFunds;

            var reservation = reservationRepoMock.Data.FirstOrDefault(x => x.Id == long.Parse(reserveResult.Data.Id));
            reservation.Should().NotBeNull();
            reservation.Amount.Should().Be(reserveAmount);
            reservation.WalletId.Should().Be(wallet.Id);
            reservation.IsCaptured.Should().BeFalse();

            var refundResult = await fundsOperations.Refund(reservation.Id);
            refundResult.IsSuccess.Should().BeTrue();
            refundResult.ErrorMsg.Should().BeEmpty();

            initialRealMoney.Should().BeGreaterThan(realMoneyAfterReservation);
            initialRealMoney.Should().Be(wallet.RealMoney);
            initialLockedMoney.Should().Be(0);
            initialLockedMoney.Should().Be(wallet.LockedFunds);
            lockedMoneyAfterReservation.Should().Be(reserveAmount);
        }

        [Test]
        public async Task Refund_ShouldReturnError_WhenNoLockedFundsLeft()
        {
            var wallet = walletRepoMock.Data.First();
            var reserveResult = await fundsOperations.Reserve(wallet.PlayerId, 5);
            var reservation = reservationRepoMock.Data.First(x => x.Id == long.Parse(reserveResult.Data.Id));

            wallet.LockedFunds = 0;

            var refundResult = await fundsOperations.Refund(reservation.Id);

            refundResult.IsSuccess.Should().BeFalse();
            refundResult.ErrorMsg.Should().Be("Insufficient locked funds to process refund");
        }

        [Test]
        public async Task Refund_ShouldReturnError_WhenAlreadyRefunded()
        {
            var wallet = walletRepoMock.Data.First();
            var reserveResult = await fundsOperations.Reserve(wallet.PlayerId, 10);
            var reservation = reservationRepoMock.Data.First(x => x.Id == long.Parse(reserveResult.Data.Id));

            var firstRefund = await fundsOperations.Refund(reservation.Id);
            firstRefund.IsSuccess.Should().BeTrue();

            var secondRefund = await fundsOperations.Refund(reservation.Id);
            secondRefund.IsSuccess.Should().BeFalse();
            secondRefund.ErrorMsg.Should().Be("Reservation already refunded");
        }

        [Test]
        public async Task Refund_ShouldReturnError_WhenReservationExpired()
        {
            var wallet = walletRepoMock.Data.First();
            var reserveResult = await fundsOperations.Reserve(wallet.PlayerId, 5);
            var reservation = reservationRepoMock.Data.First(x => x.Id == long.Parse(reserveResult.Data.Id));

            reservation.ExpiresAt = DateTime.UtcNow.AddMinutes(-20);

            var refundResult = await fundsOperations.Refund(reservation.Id);
            refundResult.IsSuccess.Should().BeFalse();
            refundResult.ErrorMsg.Should().Be("Reservation expired");
        }

        [Test]
        public async Task Refund_ShouldReturnFundsToRealAndBonus_WhenMixedReservation()
        {
            var wallet = walletRepoMock.Data.First();
            long initialReal = wallet.RealMoney;
            long initialBonus = wallet.BonusMoney;
            long reserveAmount = wallet.RealMoney + 5;

            var reserveResult = await fundsOperations.Reserve(wallet.PlayerId, reserveAmount);
            var reservation = reservationRepoMock.Data.First(x => x.Id == long.Parse(reserveResult.Data.Id));

            var refundResult = await fundsOperations.Refund(reservation.Id);
            refundResult.IsSuccess.Should().BeTrue();

            wallet.RealMoney.Should().Be(reserveAmount);
            wallet.BonusMoney.Should().Be(initialBonus - 5);
            wallet.LockedFunds.Should().Be(0);
        }

        [Test]
        public async Task Reserve_ShouldAccumulateLockedFundsAcrossMultipleReservations()
        {
            var wallet = walletRepoMock.Data.First();
            long firstAmount = 5;
            long secondAmount = 3;

            await fundsOperations.Reserve(wallet.PlayerId, firstAmount);
            await fundsOperations.Reserve(wallet.PlayerId, secondAmount);

            wallet.LockedFunds.Should().Be(firstAmount + secondAmount);
            reservationRepoMock.Data.Count.Should().Be(2);
        }
    }
}