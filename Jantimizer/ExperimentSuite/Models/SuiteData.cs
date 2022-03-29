﻿using DatabaseConnector;
using Histograms;
using Histograms.Models;
using QueryOptimiser;
using QueryParser;
using QueryParser.QueryParsers;
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
        public IDbConnector Connector { get; set; }
        public IPlanParser Parser { get; set; }

        public IHistogramManager<IHistogram, IDbConnector> HistoManager { get; set; }

        public IQueryOptimiser<IHistogram, IDbConnector> Optimiser { get; set; }
        public IParserManager QueryParserManager { get; set; }

        public SuiteData(
            TestSettings settings,
            string name, 
            IDbConnector connector, 
            IPlanParser parser, 
            IHistogramManager<IHistogram, IDbConnector> histoManager, 
            IQueryOptimiser<IHistogram, IDbConnector> optimiser, 
            IParserManager queryParserManager)
        {
            Settings = settings;
            Name = name;
            Connector = connector;
            Parser = parser;
            HistoManager = histoManager;
            Optimiser = optimiser;
            QueryParserManager = queryParserManager;
        }
    }
}