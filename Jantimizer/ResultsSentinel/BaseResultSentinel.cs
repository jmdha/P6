using ResultsSentinel.SentinelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsSentinel
{
    public enum SentinelCriticality { None, Low, Medium, High }
    public abstract class BaseResultSentinel<T> where T : notnull
    {
        public SentinelCriticality Criticality { get; set; }
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
                else
                    _dict.Add(hash, value);
            }
        }

        public abstract string GetErrorDescription(T value1, T value2);
    }
}
