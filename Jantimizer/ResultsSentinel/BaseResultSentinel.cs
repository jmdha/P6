using ResultsSentinel.SentinelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsSentinel
{
    public abstract class BaseResultSentinel<T> : IResultSentinel
        where T : notnull, ICloneable
    {
        public SentinelCriticality Criticality { get; internal set; }
        public bool IsEnabled { get; set; } = false;
        public List<SentinelLogItem> SentinelLog { get; }
        internal Dictionary<int, T> _dict;
        public BaseResultSentinel()
        {
            _dict = new Dictionary<int, T>();
            SentinelLog = new List<SentinelLogItem>();
        }

        public void CheckResult(T value, string caseName, string experimentName, string runnerName)
        {
            if (IsEnabled)
            {
                var hash = HashCode.Combine(caseName, experimentName, runnerName);
                if (_dict.ContainsKey(hash))
                {
                    var saved = _dict[hash];
                    if (saved.GetHashCode() != value.GetHashCode())
                        SentinelLog.Add(new SentinelLogItem(
                            Criticality,
                            experimentName, 
                            runnerName, 
                            caseName, 
                            this.GetType().Name,
                            GetErrorDescription(saved, value)
                            ));
                }
                else {
                    var clone = value.Clone();
                    if (clone is T cloneValue)
                        _dict.Add(hash, cloneValue);
                }
            }
        }

        public void ClearSentinel()
        {
            SentinelLog.Clear();
            _dict.Clear();
        }

        public abstract string GetErrorDescription(T value1, T value2);
    }
}
