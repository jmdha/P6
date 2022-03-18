using MySqlConnector;
using System.Data;
using Tools.Models;

namespace DatabaseConnector.Connectors
{
    public class MySqlConnector : BaseDbConnector<MySqlConnection, MySqlCommand, MySqlDataAdapter>
    {
        public MySqlConnector(ConnectionProperties connectionProperties) : base(connectionProperties)
        {

        }

        public override string BuildConnectionString()
        {
            return $"Server={ConnectionProperties.Server};Port={ConnectionProperties.Port};Uid={ConnectionProperties.Secrets.Username};Pwd={ConnectionProperties.Secrets.Password};Database={ConnectionProperties.Database};";
        }

        public override async Task<DataSet> AnalyseQuery(string query)
        {
            if (!query.ToUpper().Trim().StartsWith("EXPLAIN ANALYZE "))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }

        public override Task<bool> StartServer()
        {
            throw new NotImplementedException();
        }

        public override bool StopServer()
        {
            throw new NotImplementedException();
        }
    }
}
