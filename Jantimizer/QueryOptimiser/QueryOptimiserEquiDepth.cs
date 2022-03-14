using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QueryOptimiser.QueryGenerators;
using QueryOptimiser.Cost.CostCalculators;
using QueryParser;
using QueryParser.Models;
using Histograms;
using Histograms.Managers;
using DatabaseConnector;
using QueryOptimiser.Cost.Nodes;

namespace QueryOptimiser
{
    public class QueryOptimiser : IQueryOptimiser<IHistogram, IDbConnector>
    {
        public IParserManager ParserManager { get; set; }
        public IQueryGenerator QueryGenerator { get; set; }
        public IHistogramManager<IHistogram, IDbConnector> HistogramManager { get; set; }
        public ICostCalculator<IHistogram, IDbConnector> CostCalculator { get; set; }

        public QueryOptimiser(IParserManager parserManager, IQueryGenerator queryGenerator, IHistogramManager<IHistogram, IDbConnector> histogramManager)
        {
            ParserManager = parserManager;
            QueryGenerator = queryGenerator;
            HistogramManager = histogramManager;
            CostCalculator = new CostCalculatorEquiDepth(histogramManager);
        }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public string OptimiseQuery(string query)
        {
            List<INode> nodes = ParserManager.ParseQuery(query);

            return OptimiseQuery(nodes);
        }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public string OptimiseQuery(List<INode> nodes)
        {
            List<ValuedNode> valuedNodes = ValueQuery(nodes);

            return QueryGenerator.GenerateQuery(valuedNodes);
        }

        /// <summary>
        /// Calculates worst case cost of each join operation
        /// </summary>
        /// <returns></returns>
        public List<ValuedNode> ValueQuery(List<INode> nodes)
        {
            List<ValuedNode> valuedNodes = new List<ValuedNode>();
            foreach (var node in nodes)
            {
                int cost = CostCalculator.CalculateCost(node);
                valuedNodes.Add(new ValuedNode(cost, node));
            }
            return valuedNodes;
        }


    }
}
