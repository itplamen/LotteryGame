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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasOne(x => x.Wallet)
                .WithOne(x => x.Player)
                .HasForeignKey<Wallet>(x => x.PlayerId)
                .IsRequired();

            modelBuilder.Entity<Wallet>()
                .HasMany(x => x.BalanceHistories)
                .WithOne(x => x.Wallet)
                .HasForeignKey(x => x.WalletId)
                .IsRequired();

            modelBuilder.Entity<Wallet>()
                .HasIndex(x => x.PlayerId)
                .IsUnique();

            modelBuilder.Entity<BalanceHistory>()
                .HasIndex(x => x.WalletId);

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(x => x.Id);
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
                entity.ToTable(x => x
                    .HasCheckConstraint(
                        "CK_BalanceHistory_PositiveBalance",
                        "[OldBalance] >= 0 AND [NewBalance] >= 0"
                    )
                );
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
