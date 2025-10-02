namespace WagerService.Data
{
    using System.Linq.Expressions;

    using MongoDB.Driver;

    using WagerService.Data.Contracts;
    using WagerService.Data.Models;

    public class DbRepository<TEntity> : IRepository<TEntity>
       where TEntity : BaseEntity
    {
        private readonly WagerServiceDbContext dbContext;

        public DbRepository(WagerServiceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            return await dbContext.GetCollection<TEntity>()
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await dbContext.GetCollection<TEntity>()
                .Find(filter)
                .ToListAsync();
        }

        public async Task AddAsync(TEntity item)
        {
            item.CreatedOn = DateTime.UtcNow;

            await dbContext.GetCollection<TEntity>()
                .InsertOneAsync(item);
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            entity.ModifiedOn = DateTime.UtcNow;

            ReplaceOneResult result = await dbContext.GetCollection<TEntity>()
                .ReplaceOneAsync(
                    x => x.Id == entity.Id,
                    entity,
                    new ReplaceOptions { IsUpsert = false });

            return result.ModifiedCount > 0;
        }
    }
}
