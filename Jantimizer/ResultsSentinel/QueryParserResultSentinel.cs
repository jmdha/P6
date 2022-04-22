using QueryOptimiser.Models;
using QueryParser.QueryParsers;
using QueryPlanParser.Models;
using System.Text;
using Tools.Models.Dictionaries;

namespace ResultsSentinel
{
    public class QueryParserResultSentinel : BaseResultSentinel<ParserResult>
    {
        public static QueryParserResultSentinel? Instance;

        public QueryParserResultSentinel() : base()
        {
            Instance = this;
            Criticality = SentinelCriticality.Low;
        }

        public override string GetErrorDescription(ParserResult value1, ParserResult value2)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Missmatch in ParserResult!");
            sb.AppendLine("Result 1 data:");
            sb.AppendLine(value1.ToString());
            sb.AppendLine("Result 2 data:");
            sb.AppendLine(value2.ToString());

            return sb.ToString();
        }
    }
}