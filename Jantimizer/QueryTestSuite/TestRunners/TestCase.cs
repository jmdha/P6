using QueryTestSuite.Models;
using CsvHelper;

namespace QueryTestSuite.TestRunners
{
    internal class TestCase
    {
        public string Name { get; private set; } = "N/A";
        public string Category { get; private set; } = "N/A";
        public AnalysisResult AnalysisResult { get; private set; }

        public TestCase (FileInfo fileInfo, AnalysisResult analysisResult)
        {
            AnalysisResult = analysisResult;
            Name = fileInfo.Name;
            if (fileInfo.Directory != null)
                if (fileInfo.Directory.Parent != null)
                    Category = fileInfo.Directory.Parent.Name;
        }
    }
}
