using ExperimentSuite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Helpers
{
    public static class TimerHelper
    {
        public static Stopwatch GetWatchAndStart()
        {
            var watch = new Stopwatch();
            watch.Start();
            return watch;
        }

        public static TestTimeReport StopAndGetReportFromWatch(this Stopwatch watch, string expName, string dbName, string testName, string name)
        {
            watch.Stop();
            return new TestTimeReport(dbName, testName, expName, name, watch.ElapsedMilliseconds);
        }

        public static TestCaseTimeReport StopAndGetCaseReportFromWatch(this Stopwatch watch, string expName, string dbName, string testName, string caseName, string name)
        {
            watch.Stop();
            return new TestCaseTimeReport(dbName, testName, expName, caseName, name, watch.ElapsedMilliseconds);
        }
    }
}
