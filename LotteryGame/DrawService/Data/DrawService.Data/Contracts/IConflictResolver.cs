namespace DrawService.Data.Contracts
{
    using DrawService.Data.Models;

    public interface IConflictResolver<TEntity>
        where TEntity : BaseEntity
    {
        TEntity Resolve(TEntity latest, TEntity incoming);
    }
}
