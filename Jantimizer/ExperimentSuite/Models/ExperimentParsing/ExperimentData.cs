using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ExperimentSuite.Models.ExperimentParsing
{
    internal class ExperimentData
    {
        public string ExperimentName { get; set; }
        public bool RunParallel { get; set; }
        public bool RunExperiment { get; set; }
        public List<string> SetupFiles { get; set; }
        public List<string> TestFiles { get; set; }
        public List<string> ConnectorNames { get; set; }
        public string MilestoneType { get; set; }
        public JsonObject OptionalTestSettings { get; set; }

        public ExperimentData()
        {
            ExperimentName = "";
            RunParallel = false;
            RunExperiment = false;
            SetupFiles = new List<string>();
            TestFiles = new List<string>();
            ConnectorNames = new List<string>();
            MilestoneType = "";
            OptionalTestSettings = new JsonObject();
        }

        public ExperimentData(string experimentName, bool runParallel, bool runExperiment, List<string> setupFiles, List<string> testFiles, List<string> connectorName, string milestoneType, JsonObject optionalTestSettings)
        {
            ExperimentName = experimentName;
            RunParallel = runParallel;
            RunExperiment = runExperiment;
            SetupFiles = setupFiles;
            TestFiles = testFiles;
            ConnectorNames = connectorName;
            MilestoneType = milestoneType;
            OptionalTestSettings = optionalTestSettings;
        }
    }
}
