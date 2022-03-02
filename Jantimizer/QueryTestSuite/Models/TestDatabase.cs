using QueryTestSuite.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Models
{
    public abstract class TestDatabase
    {
        public string Description { get; set; }
        public DbConnector DbConnector { get; set; }
        public TestDatabase(DbConnector connector, string description)
        {
            DbConnector = connector;
            Description = description;

            Setup();
        }
        protected abstract void Setup();

        protected abstract bool ValidateSetup();

        public async Task<AnalysisResult> Analyse(Query query)
        {
            return await DbConnector.GetAnalysis(query);
        }
    }
}
