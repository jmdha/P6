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
   
        public TestCaseTimeReport(string databaseName, string testName, string experimentName, string caseName, string timerName, decimal ns, decimal ms) : base(databaseName, testName, experimentName, timerName, ns, ms)        {
            CaseName = caseName;
        }

        public TestCaseTimeReport() : base()
        {
            CaseName = "";
        }
    }
}
