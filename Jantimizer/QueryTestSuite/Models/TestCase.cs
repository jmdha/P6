using CsvHelper;
using QueryPlanParser.Models;

namespace QueryTestSuite.Models
{
    internal class TestCase
    {
        public string Name { get; private set; } = "N/A";
        public string Category { get; private set; } = "N/A";
        public AnalysisResult TestResult { get; private set; }
        public AnalysisResult JantimiserResult { get; private set; }

        public TestCase (FileInfo fileInfo, AnalysisResult analysisResult, AnalysisResult jantimiserResult)
        {
            TestResult = analysisResult;
            JantimiserResult = jantimiserResult;
            Name = fileInfo.Name;
            if (fileInfo.Directory != null)
                if (fileInfo.Directory.Parent != null)
                    Category = fileInfo.Directory.Parent.Name;
        }
    }
}
