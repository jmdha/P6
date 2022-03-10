using Npgsql;
using DatabaseConnector.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace DatabaseConnector.Connectors
{
    public class PostgreSqlConnector : DbConnector
    {
        public PostgreSqlConnector(string connectionString) : base(connectionString)
        {

        }

        public override async Task<DataSet> CallQuery(string query)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand(query, conn))
            using (var sqlAdapter = new NpgsqlDataAdapter(cmd))
            {
                DataSet dt = new DataSet();
                await Task.Run(() => sqlAdapter.Fill(dt));

                return dt;
            }
        }

        public override async Task<DataSet> AnalyseQuery(string query)
        {
            if (!query.ToUpper().Contains("EXPLAIN ANALYZE"))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }
    }
}
