

using Npgsql;
using SchoolPilot.Data.Interfaces;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;

namespace SchoolPilot.Data.Postgres.Core.Factories
{
    public class PostgresDatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly ReadOnlyDictionary<string, string> _connectionStrings;

        public PostgresDatabaseConnectionFactory()
        {
            var connectionStrings = System.Configuration.ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>()
                .ToDictionary(connectionStringSettings => connectionStringSettings.Name, connectionStringSettings => connectionStringSettings.ConnectionString);

            _connectionStrings = new ReadOnlyDictionary<string, string>(connectionStrings);
        }

        public IDbConnection GetConnection(string connectionStringName = "SchoolPilotContext")
        {
            if (!_connectionStrings.TryGetValue(connectionStringName, out var connectionString))
            {
                throw new KeyNotFoundException($"Connection string with name '{connectionStringName}' not found.");
            }

            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
