using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public interface IDbConnector
    {
        public string ConnectionString { get; set; }

        public bool CheckConnection();

        public Task<DataSet> CallQuery(FileInfo sqlFile);
        public Task<DataSet> CallQuery(string query);

        public Task<DataSet> AnalyseQuery(FileInfo sqlFile);
        public Task<DataSet> AnalyseQuery(string query);
    }
}
