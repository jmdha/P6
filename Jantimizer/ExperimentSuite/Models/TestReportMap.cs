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
            Map(m => m.OptimiserPredicted).Name("Optimiser Est. Rows");
            Map(m => m.DatabaseActual).Name("Actual Rows");
        }
    }
}