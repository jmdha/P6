using QueryOptimiser.Cost.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryOptimiser.Models
{
    public class OptimiserResult : ICloneable
    {
        public ulong EstTotalCardinality { get; set; }
        public List<INode> FromNodes { get; set; }
        public string OptimiserName { get; set; }
        public string HistogramManagerName { get; set; }
        public string EstimateCalculatorName { get; set; }

        public OptimiserResult(ulong estTotalCardinality, List<INode> fromNodes, string optimiserName, string histogramManagerName, string estimateCalculatorName)
        {
            EstTotalCardinality = estTotalCardinality;
            FromNodes = fromNodes;
            OptimiserName = optimiserName;
            HistogramManagerName = histogramManagerName;
            EstimateCalculatorName = estimateCalculatorName;
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Optimiser Name: {OptimiserName}");
            sb.AppendLine($"Histogram Manager Name: {HistogramManagerName}");
            sb.AppendLine($"Estimator Name: {EstimateCalculatorName}");
            sb.AppendLine($"Est Total Cardinality: {EstTotalCardinality}");
            sb.AppendLine($"Nodes:");
            foreach(var node in FromNodes)
                sb.AppendLine($"\t {node}");

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EstTotalCardinality, OptimiserName, HistogramManagerName, EstimateCalculatorName);
        }

        public object Clone()
        {
            var newList = new List<INode>();
            foreach (var node in FromNodes)
                if (node.Clone() is INode clone)
                    newList.Add(clone);
            return new OptimiserResult(EstTotalCardinality, newList, OptimiserName, HistogramManagerName, EstimateCalculatorName);
        }
    }
}
