using CsvHelper;
using CsvHelper.Configuration.Attributes;
using QueryOptimiser.Models;
using QueryPlanParser.Models;
using System.IO;

namespace ExperimentSuite.Models
{
    public class TestCaseResult
    {
        public string Name { get; private set; } = "N/A";
        public string Category { get; private set; } = "N/A";

        public SuiteData RunData { get; private set; }

        [Name("Db")]
        /// <summary> Result from the analysis query to the database system. </summary>
        public AnalysisResult DbAnalysisResult { get; private set; }
        [Name("Jantimiser")]
        /// <summary> Result from our optimiser estimates. </summary>
        public OptimiserResult JantimiserResult { get; private set; }

        public TestCaseResult(string name, string category, FileInfo caseInfo, SuiteData runData, AnalysisResult dbAnalysisResult, OptimiserResult jantimiserResult)
        {
            Name = name;
            Category = category;
            RunData = runData;
            DbAnalysisResult = dbAnalysisResult;
            JantimiserResult = jantimiserResult;
            Name = caseInfo.Name;
        }
    }
}
