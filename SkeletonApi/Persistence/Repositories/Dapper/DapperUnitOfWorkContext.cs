using Microsoft.Extensions.Configuration;
using SkeletonApi.Persistence.Interfaces;
using System.Data;

namespace SkeletonApi.Persistence.Repositories.Dapper;

public class DapperUnitOfWorkContext : IDapperCreateUnitOfWork, IGetConnection
{
    private readonly string _connectionString;
    private DapperUnitOfWork? _unitOfWork;

    private bool IsUnitOfWorkOpen => !(_unitOfWork == null || _unitOfWork.IsDisposed);

    public DapperUnitOfWorkContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("sqlConnection");
    }

    public IDbConnection GetConnection()
    {
        if (!IsUnitOfWorkOpen)
        {
            throw new InvalidOperationException(
                "There is not current unit of work from which to get a connection. Call Create first");
        }
        //Console.WriteLine("SEKALI APA DUA KALI");
        return _unitOfWork!.Connection;
    }

    public IDapperUnitOfWork Create()
    {
        if (IsUnitOfWorkOpen)
        {
            throw new InvalidOperationException(
                "Cannot begin a transaction before the unit of work from the last one is disposed");
        }

        _unitOfWork = new DapperUnitOfWork(_connectionString);
        return _unitOfWork;
    }
}