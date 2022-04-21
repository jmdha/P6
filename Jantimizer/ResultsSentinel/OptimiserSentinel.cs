using QueryOptimiser.Models;
using ResultsSentinel.Exceptions;
using Tools.Models.Dictionaries;

namespace ResultsSentinel
{
    public class OptimiserSentinel
    {
        internal Dictionary<int, OptimiserResult> _dict;
        public static OptimiserSentinel Instance;

        public OptimiserSentinel()
        {
            _dict = new Dictionary<int, OptimiserResult>();
            Instance = this;
        }

        public void CheckResult(OptimiserResult value)
        {
            var hash = value.GetHashCode();
            if (_dict.ContainsKey(hash))
            {
                var saved = _dict[hash];
                if (saved.EstTotalCardinality != value.EstTotalCardinality)
                    throw new OptimiserSentinelErrorLogException(new CardinalityMismatchException("Error! Two OptimiserResult's was not the same!"), saved, value);
            }
            else
                _dict.Add(hash, value);
        }
    }
}