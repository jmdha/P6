using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace QueryPlanParser.Models
{
    public class AnalysisResultQueryTree : ICloneable
    {
        public string Name { get; }
        public decimal? EstimatedCost { get; }
        public ulong? EstimatedCardinality { get; }
        public ulong? ActualCardinality { get; set; }
        public TimeSpan? ActualTime { get; set; }
        [Ignore]
        public List<AnalysisResultQueryTree> SubQueries { get; }

        public AnalysisResultQueryTree(string name, decimal? estimatedCost, ulong? estimatedCardinality, ulong? actualCardinality, TimeSpan? actualTime)
        {
            Name = name;
            EstimatedCost = estimatedCost;
            EstimatedCardinality = estimatedCardinality;
            ActualCardinality = actualCardinality;
            ActualTime = actualTime;
            SubQueries = new List<AnalysisResultQueryTree>();
        }

        public override string ToString()
        {
            return BuildStringBuilderRec(new StringBuilder(), 0).ToString();
        }

        internal StringBuilder BuildStringBuilderRec(StringBuilder sb, int indentLevel)
        {
            sb.Append(new string('\t', indentLevel));
            sb.AppendLine(Name);
            foreach(var sub in SubQueries)
                sub.BuildStringBuilderRec(sb, indentLevel+1);

            return sb;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach(var sub in SubQueries)
                hash += sub.GetHashCode();
            return hash + HashCode.Combine(Name, EstimatedCost, EstimatedCardinality, ActualCardinality, ActualTime);
        }

        public object Clone()
        {
            var newTree = new AnalysisResultQueryTree(Name, EstimatedCost, EstimatedCardinality, ActualCardinality, ActualTime);
            foreach (AnalysisResultQueryTree sub in SubQueries)
                if (sub.Clone() is AnalysisResultQueryTree clone)
                    newTree.SubQueries.Add(clone);
            return newTree;
        }
    }
}
