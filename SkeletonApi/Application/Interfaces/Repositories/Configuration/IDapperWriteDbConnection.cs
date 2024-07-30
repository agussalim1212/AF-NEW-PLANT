using System.Data;

namespace SkeletonApi.Application.Interfaces.Repositories.Configuration
{
    public interface IDapperWriteDbConnection : IDapperReadDbConnection
    {
        Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
    }
}