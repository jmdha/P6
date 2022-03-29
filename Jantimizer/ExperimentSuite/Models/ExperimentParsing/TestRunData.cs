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
        public List<string> TestFiles { get; set; }

        public TestRunData(string connectorName, List<string> testFiles)
        {
            ConnectorName = connectorName;
            TestFiles = testFiles;
        }
    }
}
