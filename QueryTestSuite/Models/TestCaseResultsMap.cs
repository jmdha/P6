using CsvHelper;
using QueryPlanParser.Models;
using CsvHelper.Configuration.Attributes;
using CsvHelper.Configuration;

namespace QueryTestSuite.Models {
    internal sealed class TestCaseResultMap : ClassMap<TestCaseResult>
    {
        public TestCaseResultMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Category).Name("Category");
            Map(m => m.DbAnalysisResult.Name).Name("DbName");
            Map(m => m.DbAnalysisResult.EstimatedCost).Name("DbEstimatedCost");
            Map(m => m.DbAnalysisResult.EstimatedCardinality).Name("DbEstimatedCardinality");
            Map(m => m.DbAnalysisResult.ActualCardinality).Name("DbActualCardinality");
            Map(m => m.DbAnalysisResult.ActualTime).Name("DbActualTime");
            Map(m => m.JantimiserResult.Name).Name("JantimiserName");
            Map(m => m.JantimiserResult.EstimatedCost).Name("JantimiserEstimatedCost");
            Map(m => m.JantimiserResult.EstimatedCardinality).Name("JantimiserEstimatedCardinality");
        }
    }
}