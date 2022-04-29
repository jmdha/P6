using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryEstimator;
using QueryPlanParser;
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

        public IHistogramManager HistoManager { get; set; }

        public IQueryEstimator<JsonQuery> Estimator { get; set; }

        public SuiteData(
            TestSettings settings,
            string id,
            string name, 
            IDbConnector connector, 
            IPlanParser parser, 
            IHistogramManager histoManager,
            IQueryEstimator<JsonQuery> estimator)
        {
            Settings = settings;
            ID = id;
            Name = name;
            Connector = connector;
            Parser = parser;
            HistoManager = histoManager;
            Estimator = estimator;
        }
    }
}
