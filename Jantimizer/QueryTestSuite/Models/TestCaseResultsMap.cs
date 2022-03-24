using CsvHelper;
using QueryPlanParser.Models;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;

namespace QueryTestSuite.Models {
    internal sealed class TestCaseResultMap : ClassMap<TestCaseResult>
    {
        public TestCaseResultMap()
        {
            Map(m => m.Name).Name("Case");
            Map(m => m.RunData.Name).Name("Database");
            Map(m => m.Category).Name("Category");
            Map(m => m.DbAnalysisResult.EstimatedCardinality).Name("Database Est. Rows");
            Map(m => m.JantimiserResult.EstTotalCardinality).Name("Optimiser Est. Rows");
            Map(m => m.DbAnalysisResult.ActualCardinality).Name("Actual Rows");
            Map(m => m.DbAnalysisResult.ActualTime).Name("Database Act. Time");
        }
    }
}