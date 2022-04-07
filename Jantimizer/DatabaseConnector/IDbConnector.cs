using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace DatabaseConnector
{
    public interface IDbConnector
    {
        public string Name { get; set; }
        public ConnectionProperties ConnectionProperties { get; set; }

        public bool CheckConnection();

        public string BuildConnectionString();

        public Task<DataSet> CallQueryAsync(FileInfo sqlFile);
        public Task<DataSet> CallQueryAsync(string query);
        public DataSet CallQuery(FileInfo sqlFile);
        public DataSet CallQuery(string query);

        public Task<DataSet> AnalyseQueryAsync(FileInfo sqlFile);
        public Task<DataSet> AnalyseQueryAsync(string query);
        public DataSet AnalyseQuery(FileInfo sqlFile);
        public DataSet AnalyseQuery(string query);

        public Task<DataSet> ExplainQueryAsync(FileInfo sqlFile);
        public Task<DataSet> ExplainQueryAsync(string query);
        public DataSet ExplainQuery(FileInfo sqlFile);
        public DataSet ExplainQuery(string query);

        public Task<DataSet> AnalyseExplainQueryAsync(FileInfo sqlFile);
        public Task<DataSet> AnalyseExplainQueryAsync(string query);
        public DataSet AnalyseExplainQuery(FileInfo sqlFile);
        public DataSet AnalyseExplainQuery(string query);
    }
}
