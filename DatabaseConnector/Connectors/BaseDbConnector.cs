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
        public ConnectionProperties ConnectionProperties { get; set; }

        public BaseDbConnector(ConnectionProperties connectionProperties)
        {
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

        public async Task<DataSet> CallQuery(string query)
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
                        await Task.Run(() => sqlAdapter.Fill(dt));

                        return dt;
                    }
                }
            }
        }

        public Task<DataSet> CallQuery(FileInfo sqlFile) => CallQuery(File.ReadAllText(sqlFile.FullName));

        public Task<DataSet> AnalyseQuery(FileInfo sqlFile) => AnalyseQuery(File.ReadAllText(sqlFile.FullName));
        public abstract Task<DataSet> AnalyseQuery(string query);
    }
}
