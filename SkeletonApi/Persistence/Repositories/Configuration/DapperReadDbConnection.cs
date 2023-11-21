using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using SkeletonApi.Application.Interfaces.Repositories;
using System.Data;


namespace SkeletonApi.Persistence.Repositories.Configuration
{
    public class DapperReadDbConnection : IDapperReadDbConnection, IDisposable
    {
        private readonly NpgsqlConnection connection;

        public DapperReadDbConnection(IConfiguration configuration)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("sqlConnection"));
            using var dataSource = dataSourceBuilder.Build();
            connection = dataSource.OpenConnection();
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        {
            return (await connection.QueryAsync<T>(sql, param, transaction)).AsList();
        }

        public async Task<T> QueryFirstOrDefaultaAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        public async Task<IEnumerable<TResult>> QueryMapAsync<T1, T2, TResult>(string sql, Func<T1, T2, TResult> map, object? param = null, IDbTransaction? transaction = null, string splitOn = "Id", CancellationToken cancellationToken = default)
        {
            return await connection.QueryAsync(sql, map, param, transaction, true, splitOn);
        }

        public async Task<IEnumerable<TResult>> QueryMapAsync<T1, T2, T3, TResult>(string sql, Func<T1, T2, T3, TResult> map, object? param = null, IDbTransaction? transaction = null, string splitOn = "Id", CancellationToken cancellationToken = default)
        {
            return await connection.QueryAsync(sql, map, param, transaction, true, splitOn);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        {
            return await connection.QuerySingleAsync<T>(sql, param, transaction);
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
