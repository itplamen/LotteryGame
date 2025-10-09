namespace DrawService.Tests.Utils.Database
{
    using System.Linq.Expressions;

    using Moq;

    using DrawService.Data.Contracts;
    using DrawService.Data.Models;

    public class DbRepositoryMock<TEntity>
        where TEntity : BaseEntity, new()
    {
        private readonly Mock<IRepository<TEntity>> repository;
        public IRepository<TEntity> Mock => repository.Object;

        public List<TEntity> Data { get; private set; }

        public DbRepositoryMock(IEnumerable<TEntity> initialData = null)
        {
            repository = new Mock<IRepository<TEntity>>();
            Setup(initialData);
        }

        private void Setup(IEnumerable<TEntity> initialData = null)
        {
            Data = initialData?.ToList() ?? new List<TEntity>();

            repository.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => Data.FirstOrDefault(x => x.Id == id));

            repository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<TEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<TEntity, bool>> filter) =>
                    Data.AsQueryable().Where(filter).ToList());

            repository.Setup(x => x.AddAsync(It.IsAny<TEntity>()))
                .ReturnsAsync((TEntity entity) =>
                {
                    if (string.IsNullOrEmpty(entity.Id))
                    {
                        entity.Id = Guid.NewGuid().ToString();
                    }

                    Data.Add(entity);
                    return entity;
                });

            repository.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<TEntity>>()))
                .ReturnsAsync((IEnumerable<TEntity> entities) =>
                {
                    foreach (var entity in entities)
                    {
                        if (string.IsNullOrEmpty(entity.Id))
                        {
                            entity.Id = Guid.NewGuid().ToString();
                        }

                        Data.Add(entity);
                    }
                    return entities;
                });

            repository.Setup(x => x.UpdateAsync(It.IsAny<TEntity>()))
                .ReturnsAsync((TEntity entity) =>
                {
                    var index = Data.FindIndex(x => x.Id == entity.Id);
                    if (index >= 0)
                    {
                        Data[index] = entity;
                    }
                    else
                    {
                        Data.Add(entity);
                    }

                    return entity;
                });
        }

        public void SetupGetById(string id, TEntity entity)
        {
            var existing = Data.FirstOrDefault(x => x.Id == id);
            if (existing != null)
            {
                Data.Remove(existing);
            }

            Data.Add(entity);

            repository.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(entity);
        }
    }
}
