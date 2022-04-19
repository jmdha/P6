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
    public abstract class BaseDbConnector<Connector, Command> : IDbConnector, IDisposable
        where Connector : DbConnection
        where Command : DbCommand
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
            using (var conn = GetConnector(GetConnectionString()))
            {
                try
                {
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
            using (var conn = GetConnector(GetConnectionString()))
            {
                try
                {
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

        internal abstract Connector GetConnector(string connectionString);
        internal abstract Command GetCommand(string query, Connector conn);

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
                dt.EnforceConstraints = false;
                await using (var conn = GetConnector(GetConnectionString()))
                {
                    await conn.OpenAsync();
                    await using (var cmd = GetCommand(query, conn))
                    {
                        cmd.CommandText = query;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            do
                            {
                                dt.Tables.Add();
                                dt.Tables[dt.Tables.Count - 1].Load(reader);
                            }
                            while (!reader.IsClosed);
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
                using (var conn = GetConnector(GetConnectionString()))
                {
                    conn.Open();
                    using (var cmd = GetCommand(query, conn))
                    {
                        cmd.CommandText = query;
                        using (var reader = cmd.ExecuteReader())
                        {
                            do
                            {
                                dt.Tables.Add();
                                dt.Tables[dt.Tables.Count - 1].Load(reader);
                            }
                            while (!reader.IsClosed);
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

        public abstract void Dispose();

        #endregion
    }
}
