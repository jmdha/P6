using CsvHelper.Configuration;

namespace ExperimentSuite.Models {
    public sealed class TestTimeReportMap : ClassMap<TestTimeReport>
    {
        public TestTimeReportMap()
        {
            Map(m => m.ExperimentName).Name("Experiment Name");
            Map(m => m.DatabaseName).Name("Database Name");
            Map(m => m.TestName).Name("Test Name");
            Map(m => m.TimerName).Name("Timer Name");
            Map(m => m.TimeMs).Name("Ms");
        }
    }
}