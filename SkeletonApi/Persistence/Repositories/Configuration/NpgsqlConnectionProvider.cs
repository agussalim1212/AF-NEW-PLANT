using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace Repository.Configuration
{
    public interface IConnectionProvider<TConnection>
        where TConnection : IDbConnection
    {
        Task<TConnection> OpenConnection();
    }

    public class NpgsqlConnectionProvider : IConnectionProvider<NpgsqlConnection>
    {
        private readonly string _connectionString;
        private readonly ILogger<NpgsqlConnectionProvider> _logger;

        static NpgsqlConnectionProvider()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public NpgsqlConnectionProvider(string connectionString, ILogger<NpgsqlConnectionProvider> logger)
        {
            _connectionString = connectionString;
            _logger = logger;

            if (!connectionString.Contains("Max Auto Prepare", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Connection string doesn't contain 'Max Auto Prepare', which means Dapper won't take advantage of prepared statements; See source code for more info");
            }
        }

        public async Task<NpgsqlConnection> OpenConnection()
        {
            NpgsqlConnection? connection = null;

            try
            {
                connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                return connection;
            }
            catch
            {
                if (connection is not null)
                    await connection.DisposeAsync();
                throw;
            }
        }
    }
}