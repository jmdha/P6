using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser;
using QueryPlanParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.Models
{
    public class SuiteData
    {
        public TestSettings Settings { get; set; }

        public string Name { get; set; }
        public string ID { get; set; }
        public IDbConnector Connector { get; set; }
        public IPlanParser Parser { get; set; }

        public IHistogramManager HistoManager { get; set; }

        public IQueryOptimiser Optimiser { get; set; }

        public SuiteData(
            TestSettings settings,
            string id,
            string name, 
            IDbConnector connector, 
            IPlanParser parser, 
            IHistogramManager histoManager, 
            IQueryOptimiser optimiser)
        {
            Settings = settings;
            ID = id;
            Name = name;
            Connector = connector;
            Parser = parser;
            HistoManager = histoManager;
            Optimiser = optimiser;
        }
    }
}
