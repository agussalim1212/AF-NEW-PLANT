using SkeletonApi.Domain.Common.Abstracts;

namespace SkeletonApi.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseAuditableEntity;

        IGenRepository<T> Repo<T>() where T : BaseManyToMany;

        IDataRepository<T> Data<T>();

        Task<int> Save(CancellationToken cancellationToken);

        Task<int> SaveAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}