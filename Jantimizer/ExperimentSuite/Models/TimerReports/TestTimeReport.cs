using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class TestTimeReport
    {
        public string DatabaseName { get; set; }
        public string TestName { get; set; }
        public string ExperimentName { get; set; }
        public string TimerName { get; set; }
        public decimal TimeNs { get; set; }
        public decimal TimeMs { get; set; }

        public TestTimeReport()
        {
            DatabaseName = "";
            TestName = "";
            ExperimentName = "";
            TimerName = "";
            TimeNs = 0;
            TimeMs = 0;
        }

        public TestTimeReport(string databaseName, string testName, string experimentName, string timerName, decimal timeNs, decimal timeMs)
        {
            DatabaseName = databaseName;
            TestName = testName;
            ExperimentName = experimentName;
            TimerName = timerName;
            TimeNs = timeNs;
            TimeMs = timeMs;
        }
    }
}
