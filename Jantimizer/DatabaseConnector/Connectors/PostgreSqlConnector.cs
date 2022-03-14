using Npgsql;
using System.Data;

namespace DatabaseConnector.Connectors
{
    public class PostgreSqlConnector : BaseDbConnector<NpgsqlConnection, NpgsqlCommand, NpgsqlDataAdapter>
    {
        public PostgreSqlConnector(string connectionString) : base(connectionString)
        {

        }

        public override async Task<DataSet> AnalyseQuery(string query)
        {
            if (!query.ToUpper().Trim().StartsWith("EXPLAIN ANALYZE "))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }
    }
}
