using Npgsql;
using QueryTestSuite.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace QueryTestSuite.Connectors
{
    internal class PostgreSqlConnector : DbConnector
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
            if (!query.Contains("EXPLAIN ANALYSE"))
                return await CallQuery($"EXPLAIN ANALYSE {query}");
            return await CallQuery(query);
        }
    }
}
