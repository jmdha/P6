using CsvHelper.Configuration;

namespace ExperimentSuite.Models {
    public sealed class TestCaseTimeReportMap : ClassMap<TestCaseTimeReport>
    {
        public TestCaseTimeReportMap()
        {
            Map(m => m.ExperimentName).Name("Experiment Name");
            Map(m => m.DatabaseName).Name("Database Name");
            Map(m => m.TestName).Name("Test Name");
            Map(m => m.CaseName).Name("Case Name");
            Map(m => m.TimerName).Name("Timer Name");
            Map(m => m.TimeMs).Name("Ms");
        }
    }
}