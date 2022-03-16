using DatabaseConnector;
using Histograms;
using QueryGenerator;
using QueryOptimiser;
using QueryParser;
using QueryParser.QueryParsers;
using QueryPlanParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryTestSuite.Models
{
    internal class SuiteData
    {
        public string Name { get; set; }
        public IDbConnector Connector { get; set; }
        public IPlanParser Parser { get; set; }

        public IHistogramManager<IHistogram, IDbConnector> HistoManager { get; set; }

        public IQueryOptimiser<IHistogram, IDbConnector> Optimiser { get; set; }
        public IParserManager QueryParserManager { get; set; }

        public IQueryGenerator Generator { get; set; }

        public SuiteData(string name, IDbConnector connector, IPlanParser parser, IHistogramManager<IHistogram, IDbConnector> histoManager, IQueryOptimiser<IHistogram, IDbConnector> optimiser, IParserManager queryParserManager, IQueryGenerator generator)
        {
            Name = name;
            Connector = connector;
            Parser = parser;
            HistoManager = histoManager;
            Optimiser = optimiser;
            QueryParserManager = queryParserManager;
            Generator = generator;
        }
    }
}
