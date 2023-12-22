using System.Data;

namespace SkeletonApi.Persistence.Interfaces;

public interface IGetConnection
{
    IDbConnection GetConnection();
}