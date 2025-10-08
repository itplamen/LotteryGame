namespace WalletService.Tests.Utils.Database
{
    using Moq;
    using MockQueryable;

    using WalletService.Data.Contracts;
    using WalletService.Data.Models;

    public class DbRepositoryMock<TEntity> 
        where TEntity : BaseEntity, new()
    {
        private readonly Mock<IRepository<TEntity>> repository;

        public IRepository<TEntity> Mock => repository.Object;

        public List<TEntity> Data { get; private set;  }

        public DbRepositoryMock(IEnumerable<TEntity> initialData = null)
        {
            repository = new Mock<IRepository<TEntity>>();

            Setup(initialData);
        }

        private void Setup(IEnumerable<TEntity> initialData = null)
        {
            Data = initialData?.ToList() ?? new List<TEntity> { };

            var mockQueryable = Data.BuildMock();

            repository.Setup(x => x.Filter())
                .Returns(mockQueryable);

            repository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => Data.FirstOrDefault(d => d.Id == id));

            repository.Setup(x => x.AddAsync(It.IsAny<TEntity>()))
                .Returns((TEntity entity) =>
                {
                    if (!Data.Contains(entity))
                    {
                        entity.Id = Data.LastOrDefault()?.Id ?? 1;
                        Data.Add(entity);
                    }
                    return Task.CompletedTask;
                });

            repository.Setup(x => x.Delete(It.IsAny<TEntity>()))
                .Callback<TEntity>(entity =>
                {
                    if (Data.Contains(entity))
                    {
                        entity.DeletedOn = DateTime.UtcNow;
                    }
                });

            repository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        }
    }
}
