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
            sb.Append($"CommandTimeout=13370;");

            if (ConnectionProperties.Database != null)
                sb.Append($"Database={ConnectionProperties.Database};");
            if (ConnectionProperties.Schema != null)
                sb.Append($"SearchPath={ConnectionProperties.Schema};");

            return sb.ToString();
        }


        #region AnalyseQuery

        public override async Task<DataSet> AnalyseQueryAsync(string query)
        {
            return await CallQueryAsync($"ANALYZE {RemoveExplainIfThere(query)}");
        }

        public override DataSet AnalyseQuery(string query)
        {
            return CallQuery($"ANALYZE {RemoveExplainIfThere(query)}");
        }

        #endregion

        #region ExplainQuery

        public override async Task<DataSet> ExplainQueryAsync(string query)
        {
            return await CallQueryAsync($"EXPLAIN {RemoveAnalyseIfThere(query)}");
        }

        public override DataSet ExplainQuery(string query)
        {
            return CallQuery($"EXPLAIN {RemoveAnalyseIfThere(query)}");
        }

        #endregion

        #region AnalyseExplainQuery

        public override async Task<DataSet> AnalyseExplainQueryAsync(string query)
        {
            return await CallQueryAsync($"EXPLAIN ANALYZE {RemoveExplainAnalyseIfThere(query)}");
        }

        public override DataSet AnalyseExplainQuery(string query)
        {
            return CallQuery($"EXPLAIN ANALYZE {RemoveExplainAnalyseIfThere(query)}");
        }

        #endregion

        private string RemoveExplainAnalyseIfThere(string query)
        {
            return RemoveAnalyseIfThere(RemoveExplainIfThere(query));
        }

        private string RemoveExplainIfThere(string query)
        {
            query = query.Trim();
            if (query.ToUpper().Contains(" EXPLAIN "))
                query = query.ToUpper().Replace(" EXPLAIN ", "");
            if (query.ToUpper().StartsWith("EXPLAIN "))
                query = query.ToUpper().Replace("EXPLAIN ", "");
            return query;
        }

        private string RemoveAnalyseIfThere(string query)
        {
            query = query.Trim();
            if (query.ToUpper().Contains(" ANALYZE "))
                query = query.ToUpper().Replace(" ANALYZE ", "");
            if (query.ToUpper().StartsWith("ANALYZE "))
                query = query.ToUpper().Replace("ANALYZE ", "");
            return query;
        }
    }
}
