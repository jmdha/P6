using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class TestTimeReport
    {
        public string TimerName { get; set; }
        public TimeSpan Time { get; set; }

        public TestTimeReport(string timerName, TimeSpan time)
        {
            TimerName = timerName;
            Time = time;
        }
    }
}
