using QueryOptimiser.Models;
using QueryPlanParser.Models;
using ResultsSentinel.Exceptions;
using Tools.Models.Dictionaries;

namespace ResultsSentinel
{
    public class QueryPlanParserResultSentinel
    {
        internal Dictionary<int, AnalysisResult> _dict;
        public static QueryPlanParserResultSentinel? Instance;
        public bool IsEnabled = false;

        public QueryPlanParserResultSentinel()
        {
            _dict = new Dictionary<int, AnalysisResult>();
            Instance = this;
        }

        public void CheckResult(AnalysisResult value, string caseName, string experimentName, string runnerName)
        {
            if (IsEnabled)
            {
                var hash = value.GetHashCode() + HashCode.Combine(caseName, experimentName, runnerName);
                if (_dict.ContainsKey(hash))
                {
                    var saved = _dict[hash];
                    if (saved.QueryTree.GetHashCode() != value.QueryTree.GetHashCode())
                        throw new QueryPlanParserResultSentinelErrorLogException(
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