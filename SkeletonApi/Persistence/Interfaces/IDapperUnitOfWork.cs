namespace SkeletonApi.Persistence.Interfaces;

public interface IDapperUnitOfWork : IDisposable
{
    Task RollBackAsync();

    Task CommitAsync();

    bool IsDisposed { get; }
}