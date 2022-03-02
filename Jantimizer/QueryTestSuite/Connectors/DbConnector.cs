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
    public abstract class DbConnector
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        public DbConnector(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }


        public Task<DataSet> CallQuery(FileInfo sqlFile) => CallQuery(File.ReadAllText(sqlFile.FullName));
        public abstract Task<DataSet> CallQuery(string query);


        public Task<AnalysisResult> GetAnalysis(FileInfo sqlFile) => GetAnalysis(File.ReadAllText(sqlFile.FullName));
        public abstract Task<AnalysisResult> GetAnalysis(string query);

        public async Task<ulong> GetCardinalityEstimate(string query)
        {
            var analysis = await GetAnalysis(query);

            return analysis.EstimatedCardinality;
        }

        public async Task<int> GetCardinalityActual(string query)
        {
            var result = await CallQuery(query);

            return result.Tables[0].Rows.Count;
        }

        protected static TimeSpan TimeSpanFromMs(decimal ms) => new TimeSpan((long)Math.Round(ms * 10000)); // 1 million ns per ms, but 1 tick is 100 ns, thus there are 1000000/100=10000 ticks per ms

    }
}
