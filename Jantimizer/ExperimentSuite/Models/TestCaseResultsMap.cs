using CsvHelper.Configuration;

namespace ExperimentSuite.Models {
    public sealed class TestCaseResultMap : ClassMap<TestCaseResult>
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