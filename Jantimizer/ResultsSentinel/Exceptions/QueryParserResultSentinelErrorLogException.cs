using QueryOptimiser.Models;
using QueryParser.QueryParsers;
using QueryPlanParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;

namespace ResultsSentinel.Exceptions
{
    public class QueryParserResultSentinelErrorLogException : BaseErrorLogException
    {
        public ParserResult Result1 { get; set; }
        public ParserResult Result2 { get; set; }
        public string ExperimentName { get; set; }
        public string CaseName { get; set; }
        public string RunnerName { get; set; }

        public QueryParserResultSentinelErrorLogException(ParserResult result1, ParserResult result2, string experimentName, string caseName, string runnerName) : base(new Exception())
        {
            Result1 = result1;
            Result2 = result2;
            ExperimentName = experimentName;
            CaseName = caseName;
            RunnerName = runnerName;
        }

        public override string GetErrorLogMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Missmatch in ParserResult!");
            sb.AppendLine($"Experiment Name: {ExperimentName}");
            sb.AppendLine($"Case Name: {CaseName}");
            sb.AppendLine($"Runner Name: {RunnerName}");
            sb.AppendLine("Result 1 data:");
            sb.AppendLine(Result1.ToString());
            sb.AppendLine("Result 2 data:");
            sb.AppendLine(Result2.ToString());

            return sb.ToString();
        }
    }
}
