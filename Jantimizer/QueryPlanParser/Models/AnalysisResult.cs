using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace QueryPlanParser.Models
{
    public class AnalysisResult
    {
        public string Name { get; }
        public decimal EstimatedCost { get; }
        public ulong EstimatedCardinality { get; }
        public ulong ActualCardinality { get; set; }
        public TimeSpan ActualTime { get; }
        [Ignore]
        public AnalysisResultQueryTree QueryTree { get; }

        public AnalysisResult(AnalysisResultQueryTree subTree)
        {

            Name = subTree.Name;

            EstimatedCost = subTree.EstimatedCost               ?? throw new NullReferenceException("Cost is null at top of query tree");
            EstimatedCardinality = subTree.EstimatedCardinality ?? throw new NullReferenceException("Estimated cardinality is null at top of query tree");
            ActualCardinality = subTree.ActualCardinality       ?? throw new NullReferenceException("Actual cardinality is null at top of query tree");
            ActualTime = subTree.ActualTime                     ?? throw new NullReferenceException("Actual Time is null at top of query tree");
            QueryTree = subTree;
        }

        public AnalysisResult(string name, decimal? estimatedCost, ulong? estimatedCardinality, ulong? actualCardinality, TimeSpan? actualTime)
            : this(new AnalysisResultQueryTree(name, estimatedCost, estimatedCardinality, actualCardinality, actualTime))
        { }

        public override string ToString()
        {
            return QueryTree.BuildStringBuilderRec(new StringBuilder(), 0).ToString();
        }

    }
}
