using QueryTestSuite.Models;
using CsvHelper;

namespace QueryTestSuite.TestRunners
{
    internal class TestCase
    {
        public string Name { get; private set; }
        public string Category { get; private set; }
        public AnalysisResult AnalysisResult { get; private set; }

        public TestCase (string Name, string Category)
        {
            this.Name = Name;
            this.Category = Category;
        }

        public async Task<TestCase> Run(Task<AnalysisResult> AnalysisResult)
        {
            this.AnalysisResult = await AnalysisResult;
            return this;
        }
    }
}
