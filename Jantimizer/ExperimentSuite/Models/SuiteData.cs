using DatabaseConnector;
using QueryEstimator;
using QueryPlanParser;
using Milestoner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace ExperimentSuite.Models
{
    public class SuiteData
    {
        public TestSettings Settings { get; set; }

        public string Name { get; set; }
        public string ID { get; set; }
        public IDbConnector Connector { get; set; }
        public IPlanParser Parser { get; set; }

        public IMilestoner Milestoner { get; set; }

        public IQueryEstimator<JsonQuery> Estimator { get; set; }

        public SuiteData(
            TestSettings settings,
            string id,
            string name, 
            IDbConnector connector, 
            IPlanParser parser,
            IMilestoner milestoner,
            IQueryEstimator<JsonQuery> estimator)
        {
            Settings = settings;
            ID = id;
            Name = name;
            Connector = connector;
            Parser = parser;
            Milestoner = milestoner;
            Estimator = estimator;
        }
    }
}
