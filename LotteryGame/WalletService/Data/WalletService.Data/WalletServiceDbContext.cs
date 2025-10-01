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

                entity.HasMany(x => x.BalanceHistories)
                    .WithOne(x => x.Wallet)
                    .HasForeignKey(x => x.WalletId)
                    .IsRequired();

                entity.HasIndex(x => x.PlayerId)
                    .IsUnique();

                entity.ToTable(x => 
                    x.HasCheckConstraint(
                        "CK_Wallet_PositiveBalance",
                        "[RealMoney] >= 0 AND [BonusMoney] >= 0 AND [LockedFunds] >= 0 AND [LoyaltyPoints] >= 0"
                    )
                );
            });

            modelBuilder.Entity<BalanceHistory>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.WalletId);
                entity.HasIndex(x => x.ReferenceId);

                entity.ToTable(x => x
                    .HasCheckConstraint(
                        "CK_BalanceHistory_PositiveBalance",
                        "[OldBalance] >= 0 AND [NewBalance] >= 0"
                    )
                );
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Wallet)
                      .WithMany(x => x.Reservations)
                      .HasForeignKey(x => x.WalletId)
                      .IsRequired();

                entity.Property(x => x.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(x => x.ExpiresAt)
                      .IsRequired();

                entity.Property(x => x.IsCaptured)
                      .HasDefaultValue(false);

                entity.HasIndex(x => x.WalletId);

                entity.Property(x => x.TicketId)
                    .IsRequired();

                entity.HasIndex(x => x.TicketId)
                    .IsUnique();

                entity.ToTable(x => x
                    .HasCheckConstraint(
                        "CK_Reservation_PositiveAmount",
                        "[Amount] >= 0"
                    )
                );
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
