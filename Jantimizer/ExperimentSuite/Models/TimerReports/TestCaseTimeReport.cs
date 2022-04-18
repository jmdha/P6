using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class TestCaseTimeReport : TestTimeReport
    {
        public string CaseName { get; set; }
   
        public TestCaseTimeReport(string databaseName, string testName, string experimentName, string caseName, string timerName, long time) : base(databaseName, testName, experimentName, timerName, time)
        {
            CaseName = caseName;
        }

        public TestCaseTimeReport() : base()
        {
            CaseName = "";
        }
    }
}
