namespace DrawService.Data
{
    using System.Linq.Expressions;

    using CouchDB.Driver;

    using DrawService.Data.Contracts;
    using DrawService.Data.Models;

    public class DbRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly ICouchDatabase<TEntity> database;

        public DbRepository(DrawServiceDbContext dbContext)
        {
            database = dbContext.GetDatabase<TEntity>().GetAwaiter().GetResult();
        }

        public async Task<TEntity> GetByIdAsync(string id) => await database.FindAsync(id);

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter) => await database.QueryAsync(filter);

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.CreatedOn = DateTime.UtcNow;

            TEntity created = await database.AddAsync(entity);

            return created;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            IList<TEntity> range = entities.Select(x =>
            {
                x.CreatedOn = DateTime.UtcNow;
                return x;
            }).ToList();

            IEnumerable<TEntity> created = await database.AddOrUpdateRangeAsync(range);

            return created;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity.ModifiedOn = DateTime.UtcNow;

            TEntity updated = await database.AddOrUpdateAsync(entity);

            return updated;
        }
    }
}
