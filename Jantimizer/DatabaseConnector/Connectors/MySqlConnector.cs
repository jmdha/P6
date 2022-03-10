using MySqlConnector;
using DatabaseConnector.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace DatabaseConnector.Connectors
{
    public class MySqlConnector : DbConnector
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
            if (!query.ToUpper().Contains("EXPLAIN ANALYZE"))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }
    }
}
