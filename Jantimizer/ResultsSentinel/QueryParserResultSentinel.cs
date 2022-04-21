using QueryOptimiser.Models;
using QueryParser.QueryParsers;
using QueryPlanParser.Models;
using ResultsSentinel.Exceptions;
using Tools.Models.Dictionaries;

namespace ResultsSentinel
{
    public class QueryParserResultSentinel
    {
        internal Dictionary<int, ParserResult> _dict;
        public static QueryParserResultSentinel Instance;
        public bool IsEnabled = false;

        public QueryParserResultSentinel()
        {
            _dict = new Dictionary<int, ParserResult>();
            Instance = this;
        }

        public void CheckResult(ParserResult value, string caseName, string experimentName, string runnerName)
        {
            if (IsEnabled)
            {
                var hash = value.GetHashCode() + HashCode.Combine(caseName, experimentName, runnerName);
                if (_dict.ContainsKey(hash))
                {
                    var saved = _dict[hash];
                    if (saved.GetNodesHashCode() != value.GetNodesHashCode())
                        throw new QueryParserResultSentinelErrorLogException(
                            saved,
                            value,
                            experimentName,
                            caseName,
                            runnerName);
                }
                else
                    _dict.Add(hash, value);
            }
        }
    }
}