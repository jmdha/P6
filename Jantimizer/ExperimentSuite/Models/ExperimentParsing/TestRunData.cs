using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models.ExperimentParsing
{
    internal class TestRunData
    {
        public string ConnectorName { get; set; }
        public string ConnectorID { get; set; }
        public List<string> TestFiles { get; set; }

        public TestRunData(string connectorName, string connectorID, List<string> testFiles)
        {
            ConnectorName = connectorName;
            ConnectorID = connectorID;
            TestFiles = testFiles;
        }
    }
}
