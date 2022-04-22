using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultsSentinel.SentinelLog
{
    public class SentinelLogItem
    {
        public SentinelCriticality Criticality { get; set; }
        public string ExperimentName { get; set; }
        public string CaseName { get; set; }
        public string RunnerName { get; set; }
        public string SentinelName { get; set; }
        public string SentinelDescription { get; set; }

        public SentinelLogItem(SentinelCriticality criticality, string experimentName, string caseName, string runnerName, string sentinelName, string sentinelDescription)
        {
            Criticality = criticality;
            ExperimentName = experimentName;
            CaseName = caseName;
            RunnerName = runnerName;
            SentinelName = sentinelName;
            SentinelDescription = sentinelDescription;
        }
    }
}
