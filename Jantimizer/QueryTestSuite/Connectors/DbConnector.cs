using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresTestSuite.Connectors
{
    internal abstract class DbConnector
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        public DbConnector(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }

        public abstract Task<DataSet> CallQuery(string query);
        public abstract Task<AnalysisResult> GetAnalysis(string query);

        public async Task<int> GetCardinalityEstimate(string query)
        {
            var analysis = await GetAnalysis(query);

            return analysis.EstimatedCardinality;
        }

        public async Task<int> GetCardinalityActual(string query)
        {
            var result = await CallQuery(query);

            return result.Tables[0].Rows.Count;
        }

    }
}
