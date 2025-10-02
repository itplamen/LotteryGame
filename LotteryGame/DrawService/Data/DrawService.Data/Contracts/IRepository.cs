namespace DrawService.Data.Contracts
{
    using DrawService.Data.Models;
    using System.Linq.Expressions;

    public interface IRepository<TEntity>
       where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(string id);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter);

        Task AddAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);
    }
}
