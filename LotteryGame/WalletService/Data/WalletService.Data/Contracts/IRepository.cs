namespace WalletService.Data.Contracts
{
    using WalletService.Data.Models;

    public interface IRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task AddAsync(TEntity item);

        void Update(TEntity item);

        Task SaveChangesAsync();
    }
}
