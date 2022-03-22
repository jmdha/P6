using Npgsql;
using System.Data;
using System.Diagnostics;
using System.Text;
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

            var sb = new StringBuilder();
            sb.Append($"Host={ConnectionProperties.Secrets.Server};");
            sb.Append($"Port={ConnectionProperties.Secrets.Port};");
            sb.Append($"Username={ConnectionProperties.Secrets.Username};");
            sb.Append($"Password={ConnectionProperties.Secrets.Password};");

            if (ConnectionProperties.Database != null)
                sb.Append($"Database={ConnectionProperties.Database};");
            if (ConnectionProperties.Schema != null)
                sb.Append($"SearchPath={ConnectionProperties.Schema};");

            return sb.ToString();
        }

        public override async Task<DataSet> AnalyseQuery(string query)
        {
            if (!query.ToUpper().Trim().StartsWith("EXPLAIN ANALYZE "))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }
    }
}
