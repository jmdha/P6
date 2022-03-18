using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryPlanParser.Models
{
    public class AnalysisResult
    {
        public string Name { get; }
        public decimal EstimatedCost { get; }
        public ulong EstimatedCardinality { get; }
        public ulong ActualCardinality { get; }
        public TimeSpan ActualTime { get; }
        public List<AnalysisResult> SubQueries { get; }

        public AnalysisResult(string name, decimal estimatedCost, ulong estimatedCardinality, ulong actualCardinality, TimeSpan actualTime)
        {
            Name = name;
            EstimatedCost = estimatedCost;
            EstimatedCardinality = estimatedCardinality;
            ActualCardinality = actualCardinality;
            ActualTime = actualTime;
            SubQueries = new List<AnalysisResult>();
        }

        public override string ToString()
        {
            return BuildStringBuilderRec(new StringBuilder(), 0).ToString();
        }

        private StringBuilder BuildStringBuilderRec(StringBuilder sb, int indentLevel)
        {
            sb.Append(new string('\t', indentLevel));
            sb.AppendLine(Name);
            foreach(var sub in SubQueries)
                sub.BuildStringBuilderRec(sb, indentLevel+1);

            return sb;
        }

    }
}
