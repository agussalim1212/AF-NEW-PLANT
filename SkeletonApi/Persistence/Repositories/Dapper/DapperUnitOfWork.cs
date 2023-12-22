using System;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SkeletonApi.Application.Interfaces.Repositories;
using SkeletonApi.Persistence.Interfaces;
using Dapper;

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