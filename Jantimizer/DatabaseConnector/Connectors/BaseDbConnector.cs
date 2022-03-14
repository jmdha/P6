using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Connectors
{
    public abstract class BaseDbConnector<Connector, Command, Adapter> : IDbConnector
        where Connector : DbConnection, new()
        where Command : DbCommand, new()
        where Adapter : DbDataAdapter, new()
    {
        public string ConnectionString { get; set; }

        public BaseDbConnector(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public bool CheckConnection()
        {
            using (var conn = new Connector())
            {
                try
                {
                    conn.ConnectionString = ConnectionString;
                    conn.Open();
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
        }

        public async Task<DataSet> CallQuery(string query)
        {
            using (var conn = new Connector())
            {
                conn.ConnectionString = ConnectionString;
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
