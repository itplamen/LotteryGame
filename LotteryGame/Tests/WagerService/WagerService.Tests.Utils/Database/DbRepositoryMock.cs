namespace WagerService.Tests.Utils.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Moq;

    using WagerService.Data.Contracts;
    using WagerService.Data.Models;

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

            repository.Setup(x => x.FindAsync(It.IsAny<Expression<Func<TEntity, bool>>>()))
                .ReturnsAsync((Expression<Func<TEntity, bool>> filter) =>
                {
                    return Data.AsQueryable().Where(filter).ToList();
                });

            repository.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => Data.FirstOrDefault(d => d.Id.ToString() == id));

            repository.Setup(x => x.AddAsync(It.IsAny<TEntity>()))
                .Returns((TEntity entity) =>
                {
                    if (!Data.Contains(entity))
                    {
                        entity.Id = Guid.NewGuid().ToString();
                        Data.Add(entity);
                    }
                    return Task.CompletedTask;
                });

            repository.Setup(x => x.AddAsync(It.IsAny<IEnumerable<TEntity>>()))
                .Returns((IEnumerable<TEntity> entities) =>
                {
                    var added = new List<TEntity>();
                    foreach (var entity in entities)
                    {
                        if (!Data.Contains(entity))
                        {
                            entity.Id = Guid.NewGuid().ToString();
                            Data.Add(entity);
                            added.Add(entity);
                        }
                    }
                    return Task.FromResult<IEnumerable<TEntity>>(added);
                });

            repository.Setup(x => x.UpdateAsync(It.IsAny<TEntity>()))
                .ReturnsAsync((TEntity entity) =>
                {
                    var index = Data.FindIndex(d => d.Id == entity.Id);
                    if (index >= 0)
                    {
                        Data[index] = entity;
                        return true;
                    }
                    return false;
                });

            repository.Setup(x => x.UpdateAsync(It.IsAny<IEnumerable<TEntity>>()))
                .ReturnsAsync((IEnumerable<TEntity> entities) =>
                {
                    bool allUpdated = true;
                    foreach (var entity in entities)
                    {
                        var index = Data.FindIndex(d => d.Id == entity.Id);
                        if (index >= 0)
                        {
                            Data[index] = entity;
                        }
                        else
                        {
                            allUpdated = false;
                        }
                    }
                    return allUpdated;
                });

            
        }
    }
}
