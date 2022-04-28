using QueryPlanParser.Models;
using System.Text;
using Tools.Models.Dictionaries;

namespace ResultsSentinel
{
    public class QueryPlanParserResultSentinel : BaseResultSentinel<AnalysisResult>
    {
        public static QueryPlanParserResultSentinel? Instance;

        public QueryPlanParserResultSentinel() : base()
        {
            Instance = this;
            Criticality = SentinelCriticality.Low;
        }

        public override string GetErrorDescription(AnalysisResult value1, AnalysisResult value2)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Missmatch in AnalysisResult!");
            sb.AppendLine("Expected data:");
            sb.AppendLine(value1.ToString());
            sb.AppendLine("Actual Data:");
            sb.AppendLine(value2.ToString());

            return sb.ToString();
        }
    }
}