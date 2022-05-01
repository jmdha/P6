using CsvHelper.Configuration;

namespace ExperimentSuite.Models {
    public sealed class TestReportMap : ClassMap<TestReport>
    {
        public TestReportMap()
        {
            Map(m => m.ExperimentName).Name("Experiment");
            Map(m => m.Category).Name("Category");
            Map(m => m.DatabaseName).Name("Database");
            Map(m => m.CaseName).Name("Case");
            Map(m => m.DatabasePredicted).Name("Database Est. Rows");
            Map(m => m.EstimatorPredicted).Name("Estimator Rows");
            Map(m => m.DatabaseActual).Name("Actual Rows");
            Map(m => m.DatabaseOffBy).Name("Database off by");
            Map(m => m.EstimatorOffBy).Name("Estimator off by");
        }
    }
}