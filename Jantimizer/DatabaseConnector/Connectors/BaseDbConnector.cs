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
    public abstract class BaseDbConnector<Connector, Adapter> : IDbConnector
        where Connector : DbConnection, new()
        where Adapter : DbDataAdapter, new()
    {
        public string Name { get; set; }
        public ConnectionProperties ConnectionProperties { get; set; }
        internal string CurrentConnectionString = "";
        internal int CurrentConnectionStringHash = -1;

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
                    conn.ConnectionString = GetConnectionString();
                    conn.Open();
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
        }

        public async Task<bool> CheckConnectionAsync()
        {
            using (var conn = new Connector())
            {
                try
                {
                    conn.ConnectionString = GetConnectionString();
                    await conn.OpenAsync();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        public abstract string GetConnectionString();

        #region CallQuery

        public async Task<DataSet> CallQueryAsync(FileInfo sqlFile)
        {
            using (var reader = new StreamReader(sqlFile.FullName))
                return await CallQueryAsync(await reader.ReadToEndAsync());
        }
        public virtual async Task<DataSet> CallQueryAsync(string query)
        {
            try
            {
                DataSet dt = new DataSet();
                await using (var conn = new Connector())
                {
                    conn.ConnectionString = GetConnectionString();
                    await conn.OpenAsync();
                    await using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        
                        using (var sqlAdapter = new Adapter())
                        {
                            sqlAdapter.SelectCommand = cmd;
                            await Task.Run(() => sqlAdapter.Fill(dt));
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectorErrorLogException(ex, Name, query);
            }
        }

        public DataSet CallQuery(FileInfo sqlFile)
        {
            using (var reader = new StreamReader(sqlFile.FullName))
                return CallQuery(reader.ReadToEnd());
        }
        public virtual DataSet CallQuery(string query)
        {
            try 
            {
                DataSet dt = new DataSet();
                using (var conn = new Connector())
                {
                    conn.ConnectionString = GetConnectionString();
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;

                        using (var sqlAdapter = new Adapter())
                        {
                            sqlAdapter.SelectCommand = cmd;
                            sqlAdapter.Fill(dt);
                        }
                    }
                }
                return dt;
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
