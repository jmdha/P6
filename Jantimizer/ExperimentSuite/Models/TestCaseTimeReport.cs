using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class TestCaseTimeReport
    {
        public string CaseName { get; set; }
        public string TimerName { get; set; }
        public TimeSpan Time { get; set; }

        public TestCaseTimeReport(string caseName, string timerName, TimeSpan time)
        {
            CaseName = caseName;
            TimerName = timerName;
            Time = time;
        }
    }
}
