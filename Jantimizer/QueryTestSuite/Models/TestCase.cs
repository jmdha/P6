using CsvHelper;
using DatabaseConnector.Models;

namespace QueryTestSuite.Models
{
    internal class TestCase
    {
        public string Name { get; private set; } = "N/A";
        public string Category { get; private set; } = "N/A";
        public AnalysisResult TestResult { get; private set; }

        public TestCase (FileInfo fileInfo, AnalysisResult analysisResult)
        {
            TestResult = analysisResult;
            Name = fileInfo.Name;
            if (fileInfo.Directory != null)
                if (fileInfo.Directory.Parent != null)
                    Category = fileInfo.Directory.Parent.Name;
        }
    }
}
