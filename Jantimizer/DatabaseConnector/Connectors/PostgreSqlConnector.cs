using Npgsql;
using System.Data;
using System.Diagnostics;
using Tools.Models;

namespace DatabaseConnector.Connectors
{
    public class PostgreSqlConnector : BaseDbConnector<NpgsqlConnection, NpgsqlCommand, NpgsqlDataAdapter>
    {
        public PostgreSqlConnector(ConnectionProperties connectionProperties) : base(connectionProperties)
        {

        }

        public override string BuildConnectionString()
        {
            if (ConnectionProperties.Secrets == null)
                throw new ArgumentNullException("Error, base connection properties was not set!");
            return $"Host={ConnectionProperties.Secrets.Server};Port={ConnectionProperties.Secrets.Port};Username={ConnectionProperties.Secrets.Username};Password={ConnectionProperties.Secrets.Password};Database={ConnectionProperties.Database};SearchPath={ConnectionProperties.Schema}";
        }

        public override async Task<DataSet> AnalyseQuery(string query)
        {
            if (!query.ToUpper().Trim().StartsWith("EXPLAIN ANALYZE "))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }
    }
}
