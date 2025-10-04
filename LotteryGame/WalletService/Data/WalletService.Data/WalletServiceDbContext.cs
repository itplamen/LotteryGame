namespace WalletService.Data
{
    using Microsoft.EntityFrameworkCore;

    using WalletService.Data.Models;
    
    public class WalletServiceDbContext : DbContext
    {
        public WalletServiceDbContext(DbContextOptions<WalletServiceDbContext> options)
            : base(options) { }

        public DbSet<Player> Players { get; set; }

        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<BalanceHistory> BalanceHistories { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<ReservationTicket> ReservationTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Wallet)
                      .WithOne(x => x.Player)
                      .HasForeignKey<Wallet>(x => x.PlayerId)
                      .IsRequired();
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.RealMoney)
                    .HasColumnType("bigint")
                    .IsRequired();

                entity.Property(x => x.BonusMoney)
                    .HasColumnType("bigint")
                    .IsRequired();

                entity.Property(x => x.LockedFunds)
                    .HasColumnType("bigint")
                    .IsRequired();

                entity.Property(x => x.LoyaltyPoints)
                    .HasColumnType("int")
                    .IsRequired();

                entity.HasMany(x => x.BalanceHistories)
                      .WithOne(x => x.Wallet)
                      .HasForeignKey(x => x.WalletId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.Reservations)
                      .WithOne(x => x.Wallet)
                      .HasForeignKey(x => x.WalletId)
                      .IsRequired();

                entity.HasIndex(x => x.PlayerId)
                      .IsUnique();

                entity.ToTable(x => x.HasCheckConstraint(
                    "CK_Wallet_PositiveBalance",
                    "[RealMoney] >= 0 AND [BonusMoney] >= 0 AND [LockedFunds] >= 0 AND [LoyaltyPoints] >= 0"
                ));
            });

            modelBuilder.Entity<BalanceHistory>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.WalletId);
                entity.HasIndex(x => x.ReservationId);

                entity.Property(x => x.NewBalance)
                    .HasColumnType("bigint")
                    .IsRequired();

                entity.Property(x => x.OldBalance)
                    .HasColumnType("bigint")
                    .IsRequired();

                entity.HasOne(x => x.Wallet)
                      .WithMany(x => x.BalanceHistories)
                      .HasForeignKey(x => x.WalletId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Reservation)
                      .WithMany(r => r.BalanceHistories)
                      .HasForeignKey(x => x.ReservationId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(x => x.HasCheckConstraint(
                    "CK_BalanceHistory_PositiveBalance",
                    "[OldBalance] >= 0 AND [NewBalance] >= 0"
                ));
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Wallet)
                      .WithMany(x => x.Reservations)
                      .HasForeignKey(x => x.WalletId)
                      .IsRequired();

                entity.Property(x => x.Amount)
                    .HasColumnType("bigint")
                    .IsRequired();

                entity.Property(x => x.ExpiresAt)
                    .IsRequired();

                entity.Property(x => x.IsCaptured)
                    .HasDefaultValue(false);

                entity.HasIndex(x => x.WalletId);

                entity.HasMany(x => x.Tickets)
                      .WithOne(x => x.Reservation)
                      .HasForeignKey(x => x.ReservationId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.BalanceHistories)
                      .WithOne(x => x.Reservation)
                      .HasForeignKey(x => x.ReservationId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(x => x.HasCheckConstraint(
                    "CK_Reservation_PositiveAmount",
                    "[Amount] >= 0"
                ));
            });

            modelBuilder.Entity<ReservationTicket>()
                .HasKey(x => new { x.ReservationId, x.TicketId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
