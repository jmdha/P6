using ResultsSentinel.SentinelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsSentinel
{
    public enum SentinelCriticality { None, Low, Medium, High }

    public interface IResultSentinel
    {
        public SentinelCriticality Criticality { get; }
        public bool IsEnabled { get; set; }
        public List<SentinelLogItem> SentinelLog { get; }
    }
}
