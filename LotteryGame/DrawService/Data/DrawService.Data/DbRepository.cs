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

        public async Task AddAsync(TEntity entity)
        {
            entity.CreatedOn = DateTime.UtcNow;

            await database.AddAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            entity.ModifiedOn = DateTime.UtcNow;

            await database.AddOrUpdateAsync(entity);
        }
    }
}
