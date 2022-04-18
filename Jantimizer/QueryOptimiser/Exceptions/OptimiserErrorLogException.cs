using QueryOptimiser.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Exceptions;

namespace QueryOptimiser.Exceptions
{
    public class OptimiserErrorLogException : BaseErrorLogException
    {
        public IQueryOptimiser Optimiser { get; set; }
        public List<INode> InputNodes { get; set; }

        public OptimiserErrorLogException(Exception actualException, IQueryOptimiser optimiser, List<INode> inputNodes) : base(actualException)
        {
            InputNodes = inputNodes;
            Optimiser = optimiser;
        }

        public override string GetErrorLogMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Optimiser Name: {Optimiser.GetType().Name}");
            sb.AppendLine($"Histogram Manager Name: {Optimiser.HistogramManager.GetType().Name}");
            sb.AppendLine($"Cost Calculator Name: {Optimiser.EstimateCalculator.GetType().Name}");
            sb.AppendLine("Input Nodes:");
            foreach(var node in InputNodes)
            {
                if (node is JoinNode joinNode)
                {
                    sb.AppendLine($"\tNode [{nameof(JoinNode)}]:");
                    sb.AppendLine($"\t\t Predicate: {joinNode.Predicate}");
                    foreach(var table in joinNode.Relation.GetJoinTables())
                        sb.AppendLine($"\t\t Tables: {table.TableName}, alias: [{table.Alias}]");
                }
            }
            return sb.ToString();
        }
    }
}
