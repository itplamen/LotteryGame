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

        public async Task AddAsync(TEntity entity)
        {
            entity.CreatedOn = DateTime.UtcNow;

            await dbContext.GetCollection<TEntity>()
                .InsertOneAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            IList<TEntity> range = entities.Select(x =>
            {
                x.CreatedOn = DateTime.UtcNow;
                return x;
            }).ToList();

            await dbContext.GetCollection<TEntity>()
                .InsertManyAsync(range);

            return range;
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

        public async Task<bool> UpdateAsync(IEnumerable<TEntity> entities)
        {
            ReplaceOneResult[] results = await Task.WhenAll(
                entities.Select(entity =>
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                    return dbContext.GetCollection<TEntity>().ReplaceOneAsync(x => x.Id == entity.Id, entity);
                }));

            return results.All(x => x.ModifiedCount > 0);
        }
    }
}
