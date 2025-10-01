namespace WagerService.Data.Contracts
{
    using System.Linq.Expressions;
    
    using WagerService.Data.Models;

    public interface IRepository<TEntity>
       where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter);

        Task AddAsync(TEntity item);
    }
}
