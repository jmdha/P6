using DatabaseConnector.Exceptions;
using MySqlConnector;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using Tools.Models;

[assembly:InternalsVisibleTo("DatabaseConnectorTests")]

namespace DatabaseConnector.Connectors
{
    public class MySqlConnector : BaseDbConnector<MySqlConnection, MySqlCommand, MySqlDataAdapter>
    {
        private int _timeoutCounter = 0;
        internal int _maxTimeout = 10;
        internal int _timeoutMs = 500;

        public MySqlConnector(ConnectionProperties connectionProperties) : base(connectionProperties, "MySQL")
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
            sb.Append("default command timeout=13370420;");

            if (ConnectionProperties.Database != null)
                sb.Append($"Database={ConnectionProperties.Database};");

            return sb.ToString();
        }

        #region Overrides

        public override async Task<DataSet> CallQueryAsync(string query)
        {
            try
            {
                using (var conn = new MySqlConnection())
                {
                    conn.ConnectionString = BuildConnectionString();
                    await conn.OpenAsync();
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = query;

                        using (var sqlAdapter = new MySqlDataAdapter())
                        {
                            sqlAdapter.SelectCommand = cmd;
                            DataSet dt = new DataSet();
                            await Task.Run(() => sqlAdapter.Fill(dt));

                            return dt;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // "SSL Authentication Error" error number
                if (ex.Number == 1042)
                {
                    _timeoutCounter++;
                    if (_timeoutCounter > _maxTimeout)
                        throw new DatabaseConnectorErrorLogException(new Exception("MySQL timed out too many times with the SSL error!"), Name, query);
                    await Task.Delay(_timeoutMs);
                    return await CallQueryAsync(query);
                }
                else
                    throw new DatabaseConnectorErrorLogException(ex, Name, query);
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectorErrorLogException(ex, Name, query);
            }
        }

        public override DataSet CallQuery(string query)
        {
            try
            {
                using (var conn = new MySqlConnection())
                {
                    conn.ConnectionString = BuildConnectionString();
                    conn.Open();
                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = query;

                        using (var sqlAdapter = new MySqlDataAdapter())
                        {
                            sqlAdapter.SelectCommand = cmd;
                            DataSet dt = new DataSet();
                            sqlAdapter.Fill(dt);

                            return dt;
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // "SSL Authentication Error" error number
                if (ex.Number == 1042)
                {
                    _timeoutCounter++;
                    if (_timeoutCounter > _maxTimeout)
                        throw new DatabaseConnectorErrorLogException(new Exception("MySQL timed out too many times with the SSL error!"), Name, query);
                    Thread.Sleep(_timeoutMs);
                    return CallQuery(query);
                }
                else
                    throw new DatabaseConnectorErrorLogException(ex, Name, query);
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectorErrorLogException(ex, Name, query);
            }
        }

        #endregion

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
            return await CallQueryAsync($"EXPLAIN FORMAT=TREE {RemoveAnalyseIfThere(query)}");
        }

        public override DataSet ExplainQuery(string query)
        {
            return CallQuery($"EXPLAIN FORMAT=TREE {RemoveAnalyseIfThere(query)}");
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
