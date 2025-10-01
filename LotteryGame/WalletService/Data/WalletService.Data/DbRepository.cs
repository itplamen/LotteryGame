namespace WalletService.Data
{
    using Microsoft.EntityFrameworkCore;

    using WalletService.Data.Contracts;
    using WalletService.Data.Models;

    public class DbRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly WalletServiceDbContext context;
        private readonly DbSet<TEntity> dbSet;

        public DbRepository(WalletServiceDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("An instance of DbContext is required to use this repository.", nameof(context));
            }

            this.context = context;
            this.dbSet = this.context.Set<TEntity>();
        }

        public IQueryable<TEntity> Filter() => dbSet;

        public async Task<TEntity> GetByIdAsync(int id) => await dbSet.FindAsync(id);

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await dbSet.ToListAsync();

        public async Task AddAsync(TEntity item)
        {
            item.CreatedOn = DateTime.UtcNow;
            await dbSet.AddAsync(item);
        }

        public void Update(TEntity item) => dbSet.Update(item);

        public void Delete(TEntity item)
        {
            item.DeletedOn = DateTime.UtcNow;
            dbSet.Update(item);
        }

        public Task SaveChangesAsync() => context.SaveChangesAsync();
    }
}
