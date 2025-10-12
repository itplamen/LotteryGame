namespace DrawService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using CouchDB.Driver;
    using CouchDB.Driver.Exceptions;
    using CouchDB.Driver.Extensions;

    using DrawService.Data.Contracts;
    using DrawService.Data.Models;

    public class DbRepository<TEntity> : IRepository<TEntity>
        where TEntity : BaseEntity
    {
        private const int RETRY_COUNT = 5;

        private readonly ICouchDatabase<TEntity> database;
        private readonly IConflictResolver<TEntity> conflictResolver;

        public DbRepository(DrawServiceDbContext dbContext, IConflictResolver<TEntity> conflictResolver)
        {
            database = dbContext.GetDatabase<TEntity>().GetAwaiter().GetResult();
            this.conflictResolver = conflictResolver;
        }

        public async Task<TEntity> GetByIdAsync(string id) => await database.FindAsync(id);

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter) =>
            await database.Where(filter).ToListAsync();

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
            return await database.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            var range = entities.Select(x =>
            {
                x.CreatedOn = DateTime.UtcNow;
                return x;
            }).ToList();

            return await database.AddOrUpdateRangeAsync(range);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await Retry(async e =>
            {
                e.ModifiedOn = DateTime.UtcNow;
                return await database.AddOrUpdateAsync(e);
            },
            entity);
        }

        private async Task<TEntity> Retry(Func<TEntity, Task<TEntity>> action, TEntity entity)
        {
            for (int i = 0; i < RETRY_COUNT; i++)
            {
                try
                {
                    return await action(entity);
                }
                catch (CouchConflictException)
                {
                    var latest = await database.FindAsync(entity.Id);
                    entity = conflictResolver.Resolve(latest, entity);

                    await Task.Delay(100 * (i + 1));
                }
            }

            throw new InvalidOperationException("Retry failed");
        }
    }
}
