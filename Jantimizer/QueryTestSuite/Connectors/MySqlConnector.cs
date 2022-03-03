using MySqlConnector;
using QueryTestSuite.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace QueryTestSuite.Connectors
{
    internal class MySqlConnector : DbConnector
    {
        public MySqlConnector(string connectionString) : base(connectionString)
        {

        }

        public override async Task<DataSet> CallQuery(string query)
        {
            await using var conn = new MySqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using (var cmd = new MySqlCommand(query, conn))
            using (var sqlAdapter = new MySqlDataAdapter(cmd))
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
