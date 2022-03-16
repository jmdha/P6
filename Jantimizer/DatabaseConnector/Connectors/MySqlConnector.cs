using MySqlConnector;
using System.Data;

namespace DatabaseConnector.Connectors
{
    public class MySqlConnector : BaseDbConnector<MySqlConnection, MySqlCommand, MySqlDataAdapter>
    {
        public MySqlConnector(string connectionString) : base(connectionString)
        {

        }

        public override async Task<DataSet> AnalyseQuery(string query)
        {
            if (!query.ToUpper().Trim().StartsWith("EXPLAIN ANALYZE "))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }

        public override async Task<bool> StartServer()
        {
            throw new NotImplementedException();
        }

        public override bool StopServer()
        {
            throw new NotImplementedException();
        }
    }
}
