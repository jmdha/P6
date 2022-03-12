using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QueryParser;
using QueryParser.Models;
using Histograms;

namespace QueryOptimiser
{
    public class QueryOptimiser
    {
        public IParserManager ParserManager { get; private set; }
        public Histograms.Managers.PostgresEquiDepthHistogramManager GramManager { get; private set; }

        public QueryOptimiser(QueryParser.IParserManager parserManager, Histograms.Managers.PostgresEquiDepthHistogramManager gramManager)
        {
            ParserManager = parserManager;
            GramManager = gramManager;
        }

        public string OptimiseQuery(string query)
        {
            List<INode> nodes = ParserManager.ParseQuery(query);

            return OptimiseQuery(nodes);
        }

        public string OptimiseQuery(List<QueryParser.Models.INode> nodes)
        {
            string optimisedQuery = "";
            List<Tuple<QueryParser.Models.INode, int>> valuedNodes = ValueQuery(nodes);

            return optimisedQuery;
        }

        public List<Tuple<QueryParser.Models.INode, int>> ValueQuery(List<QueryParser.Models.INode> nodes)
        {
            List<Tuple<QueryParser.Models.INode, int>> valuedNodes = new List<Tuple<QueryParser.Models.INode, int>>();
            foreach (var node in nodes)
            {
                int cost = -1;
                if (node is QueryParser.Models.JoinNode)
                    cost = JoinCost.CalculateJoinCost((JoinNode) node, GramManager);
                valuedNodes.Add(Tuple.Create(node, cost));
            }
            return valuedNodes;
        }


    }
}
