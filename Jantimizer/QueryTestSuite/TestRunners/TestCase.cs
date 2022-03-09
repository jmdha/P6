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
            GenerateName(fileInfo);
        }

        public void GenerateName(FileInfo fileInfo) {
            Name = fileInfo.Name;
            try
            {
                Category = fileInfo.Directory.Parent.Name;
            } catch (Exception ex)
            {
                Console.WriteLine($"Could not get test caregory - {fileInfo}");
            }
        }
    }
}
