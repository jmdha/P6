using QueryOptimiser.Cost.Nodes;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class OptimiserResult
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
    }
}
