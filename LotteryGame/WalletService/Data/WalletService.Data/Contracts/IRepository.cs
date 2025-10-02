namespace WalletService.Data.Contracts
{
    using WalletService.Data.Models;

    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        public IQueryable<TEntity> Filter();

        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        Task SaveChangesAsync();

        void Delete(TEntity entity);
    }
}
