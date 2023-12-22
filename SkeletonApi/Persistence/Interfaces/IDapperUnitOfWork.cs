using System;
using System.Data;
using System.Threading.Tasks;

namespace SkeletonApi.Persistence.Interfaces;

public interface IDapperUnitOfWork : IDisposable
{
    Task RollBackAsync();
    Task CommitAsync();
    bool IsDisposed { get; }
}
