using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Histograms;
using DatabaseConnector;
using QueryParser.Models;
using QueryOptimiser.Cost.Nodes.EquiDepth;

namespace QueryOptimiser.Cost.CostCalculators
{
    public class CostCalculatorEquiDepth : ICostCalculator<IHistogram, IDbConnector>
    {
        public IHistogramManager<IHistogram, IDbConnector> HistogramManager { get; set; }

        public CostCalculatorEquiDepth(IHistogramManager<IHistogram, IDbConnector> histogramManager)
        {
            HistogramManager = histogramManager;
        }

        public int CalculateCost(INode node)
        {
            if (node is JoinNode)
                return new JoinCost().CalculateCost((JoinNode)node, HistogramManager);
            else
                throw new ArgumentException("Non handled node type " + node.ToString());
        }
    }
}
