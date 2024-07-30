using Dapper;
using Npgsql;
using SkeletonApi.Persistence.Interfaces;

namespace SkeletonApi.Persistence.Repositories.Dapper;

public class DapperUnitOfWork : IDapperUnitOfWork, IDisposable
{
    private readonly NpgsqlTransaction _transaction;
    public NpgsqlConnection Connection { get; }
    public bool IsDisposed { get; private set; } = false;

    public DapperUnitOfWork(string connectionString)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        Connection = new NpgsqlConnection(connectionString);
        Connection.Open();
        _transaction = Connection.BeginTransaction();
    }

    public async Task RollBackAsync()
    {
        await _transaction.RollbackAsync();
    }

    public async Task CommitAsync()
    {
        await _transaction.CommitAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            if (disposing)
            {
                //dispose managed resources
                _transaction?.Dispose();
                Connection?.Dispose();
            }
        }
        //dispose unmanaged resources
        IsDisposed = true;
    }
}