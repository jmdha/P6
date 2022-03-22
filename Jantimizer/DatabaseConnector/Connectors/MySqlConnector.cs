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
            if (ConnectionProperties.Secrets == null)
                throw new ArgumentNullException("Error, base connection properties was not set!");
            return $"Server={ConnectionProperties.Secrets.Server};Port={ConnectionProperties.Secrets.Port};Uid={ConnectionProperties.Secrets.Username};Pwd={ConnectionProperties.Secrets.Password};Database={ConnectionProperties.Database};";
        }

        public override async Task<DataSet> AnalyseQuery(string query)
        {
            if (!query.ToUpper().Trim().StartsWith("EXPLAIN ANALYZE "))
                return await CallQuery($"EXPLAIN ANALYZE {query}");
            return await CallQuery(query);
        }
    }
}
