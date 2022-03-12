using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QueryOptimiser.QueryGenerators;
using QueryParser;
using QueryParser.Models;
using Histograms;
using Histograms.Managers;

namespace QueryOptimiser
{
    public class QueryOptimiser
    {
        public IParserManager ParserManager { get; private set; }
        public IQueryGenerator QueryGenerator { get; private set; }
        public PostgresEquiDepthHistogramManager GramManager { get; private set; }

        public QueryOptimiser(IParserManager parserManager, IQueryGenerator queryGenerator, PostgresEquiDepthHistogramManager gramManager)
        {
            ParserManager = parserManager;
            QueryGenerator = queryGenerator;
            GramManager = gramManager;
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
            List<Tuple<INode, int>> valuedNodes = ValueQuery(nodes);

            return QueryGenerator.GenerateQuery(valuedNodes);
        }

        /// <summary>
        /// Calculates worst case cost of each join operation
        /// </summary>
        /// <returns></returns>
        public List<Tuple<INode, int>> ValueQuery(List<INode> nodes)
        {
            List<Tuple<INode, int>> valuedNodes = new List<Tuple<INode, int>>();
            foreach (var node in nodes)
            {
                int cost = -1;
                if (node is JoinNode)
                    cost = JoinCost.CalculateJoinCost((JoinNode) node, GramManager);
                valuedNodes.Add(Tuple.Create(node, cost));
            }
            return valuedNodes;
        }


    }
}
