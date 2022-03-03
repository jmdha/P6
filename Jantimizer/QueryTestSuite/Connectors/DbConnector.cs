using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Connectors
{
    public abstract class DbConnector : IDbConnector
    {
        public string ConnectionString { get; set; }

        public DbConnector(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Task<DataSet> CallQuery(FileInfo sqlFile) => CallQuery(File.ReadAllText(sqlFile.FullName));
        public abstract Task<DataSet> CallQuery(string query);

        public Task<DataSet> AnalyseQuery(FileInfo sqlFile) => AnalyseQuery(File.ReadAllText(sqlFile.FullName));
        public abstract Task<DataSet> AnalyseQuery(string query);
    }
}
