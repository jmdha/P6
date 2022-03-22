using MySqlConnector;
using System.Data;
using System.Text;
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

            var sb = new StringBuilder();
            sb.Append($"Server={ConnectionProperties.Secrets.Server};");
            sb.Append($"Port={ConnectionProperties.Secrets.Port};");
            sb.Append($"Uid={ConnectionProperties.Secrets.Username};");
            sb.Append($"Pwd={ConnectionProperties.Secrets.Password};");

            if (ConnectionProperties.Database != null)
                sb.Append($"Database={ConnectionProperties.Database};");

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
