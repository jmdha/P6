using CsvHelper;
using QueryPlanParser.Models;
using CsvHelper.Configuration.Attributes;

namespace QueryTestSuite.Models
{
    internal class TestCaseResult
    {
        public string Database { get; private set; } = "N/A";
        public string Name { get; private set; } = "N/A";
        public string Category { get; private set; } = "N/A";

        [Name("Db")]
        /// <summary> Result from the analysis query to the database system. </summary>
        public AnalysisResult DbAnalysisResult { get; private set; }
        [Name("Jantimiser")]
        /// <summary> Result from our optimiser estimates. </summary>
        public AnalysisResult JantimiserResult { get; private set; }

        public TestCaseResult (string database, FileInfo fileInfo, AnalysisResult dbAnalysisResult, AnalysisResult jantimiserResult)
        {
            Database = database;
            DbAnalysisResult = dbAnalysisResult;
            JantimiserResult = jantimiserResult;
            Name = fileInfo.Name;
            if (fileInfo.Directory != null)
                if (fileInfo.Directory.Parent != null)
                    Category = fileInfo.Directory.Parent.Name;
        }
    }
}
