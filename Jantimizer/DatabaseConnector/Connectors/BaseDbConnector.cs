using DatabaseConnector.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace DatabaseConnector.Connectors
{
    public abstract class BaseDbConnector<Connector, Command, Adapter> : IDbConnector
        where Connector : DbConnection, new()
        where Command : DbCommand, new()
        where Adapter : DbDataAdapter, new()
    {
        public string Name { get; set; }
        public ConnectionProperties ConnectionProperties { get; set; }

        public BaseDbConnector(ConnectionProperties connectionProperties, string name)
        {
            Name = name;
            ConnectionProperties = connectionProperties;
        }

        public bool CheckConnection()
        {
            using (var conn = new Connector())
            {
                try
                {
                    conn.ConnectionString = BuildConnectionString();
                    conn.Open();
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
        }


        public abstract string BuildConnectionString();

        #region CallQuery

        public async Task<DataSet> CallQueryAsync(FileInfo sqlFile) => await CallQueryAsync(File.ReadAllText(sqlFile.FullName));
        public async Task<DataSet> CallQueryAsync(string query)
        {
            try
            {
                using (var conn = new Connector())
                {
                    conn.ConnectionString = BuildConnectionString();
                    await conn.OpenAsync();
                    using (var cmd = new Command())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = query;

                        using (var sqlAdapter = new Adapter())
                        {
                            sqlAdapter.SelectCommand = cmd;
                            DataSet dt = new DataSet();
                            sqlAdapter.Fill(dt);

                            return dt;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new DatabaseConnectorErrorLogException(ex, Name, query);
            }
        }

        public DataSet CallQuery(FileInfo sqlFile) => CallQuery(File.ReadAllText(sqlFile.FullName));
        public DataSet CallQuery(string query)
        {
            try 
            { 
                using (var conn = new Connector())
                {
                    conn.ConnectionString = BuildConnectionString();
                    conn.Open();
                    using (var cmd = new Command())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = query;

                        using (var sqlAdapter = new Adapter())
                        {
                            sqlAdapter.SelectCommand = cmd;
                            DataSet dt = new DataSet();
                            sqlAdapter.Fill(dt);

                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectorErrorLogException(ex, Name, query);
            }
        }

        #endregion

        #region AnalyseQuery

        public async Task<DataSet> AnalyseQueryAsync(FileInfo sqlFile) => await AnalyseQueryAsync(File.ReadAllText(sqlFile.FullName));
        public abstract Task<DataSet> AnalyseQueryAsync(string query);
        public DataSet AnalyseQuery(FileInfo sqlFile) => AnalyseQuery(File.ReadAllText(sqlFile.FullName));
        public abstract DataSet AnalyseQuery(string query);

        #endregion

        #region ExplainQuery

        public async Task<DataSet> ExplainQueryAsync(FileInfo sqlFile) => await ExplainQueryAsync(File.ReadAllText(sqlFile.FullName));
        public abstract Task<DataSet> ExplainQueryAsync(string query);
        public DataSet ExplainQuery(FileInfo sqlFile) => ExplainQuery(File.ReadAllText(sqlFile.FullName));
        public abstract DataSet ExplainQuery(string query);

        #endregion

        #region AnalyseExplainQuery

        public async Task<DataSet> AnalyseExplainQueryAsync(FileInfo sqlFile) => await AnalyseExplainQueryAsync(File.ReadAllText(sqlFile.FullName));
        public abstract Task<DataSet> AnalyseExplainQueryAsync(string query);
        public DataSet AnalyseExplainQuery(FileInfo sqlFile) => AnalyseExplainQuery(File.ReadAllText(sqlFile.FullName));
        public abstract DataSet AnalyseExplainQuery(string query);

        #endregion
    }
}
