using CsvHelper;
using QueryPlanParser.Models;

namespace QueryTestSuite.Models
{
    internal class TestCaseResult
    {
        public string Name { get; private set; } = "N/A";
        public string Category { get; private set; } = "N/A";

        /// <summary> Result from the analysis query to the database system. </summary>
        public AnalysisResult DbAnalysisResult { get; private set; }

        /// <summary> Result from our optimiser estimates. </summary>
        public AnalysisResult JantimiserResult { get; private set; }

        public TestCaseResult (FileInfo fileInfo, AnalysisResult dbAnalysisResult, AnalysisResult jantimiserResult)
        {
            DbAnalysisResult = dbAnalysisResult;
            JantimiserResult = jantimiserResult;
            Name = fileInfo.Name;
            if (fileInfo.Directory != null)
                if (fileInfo.Directory.Parent != null)
                    Category = fileInfo.Directory.Parent.Name;
        }
    }
}
