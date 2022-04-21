using QueryOptimiser.Models;
using ResultsSentinel.Exceptions;
using Tools.Models.Dictionaries;

namespace ResultsSentinel
{
    public class OptimiserResultSentinel
    {
        internal Dictionary<int, OptimiserResult> _dict;
        public static OptimiserResultSentinel? Instance;
        public bool IsEnabled = false;

        public OptimiserResultSentinel()
        {
            _dict = new Dictionary<int, OptimiserResult>();
            Instance = this;
        }

        public void CheckResult(OptimiserResult value, string caseName, string experimentName, string runnerName)
        {
            if (IsEnabled)
            {
                var hash = value.GetHashCode() + HashCode.Combine(caseName, experimentName, runnerName);
                if (_dict.ContainsKey(hash))
                {
                    var saved = _dict[hash];
                    if (saved.EstTotalCardinality != value.EstTotalCardinality)
                        throw new OptimiserSentinelErrorLogException(
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