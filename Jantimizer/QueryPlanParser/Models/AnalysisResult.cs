using System;
using System.Collections.Generic;
using System.Data;
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
        public DataSet InputDataset { get; }
        public string ParserName { get; }

        public AnalysisResult(AnalysisResultQueryTree subTree, DataSet inputDataset, string parserName)
        {
            InputDataset = inputDataset;
            ParserName = parserName;
            Name = subTree.Name;

            EstimatedCost = subTree.EstimatedCost               ?? throw new NullReferenceException("Cost is null at top of query tree");
            EstimatedCardinality = subTree.EstimatedCardinality ?? throw new NullReferenceException("Estimated cardinality is null at top of query tree");
            ActualCardinality = subTree.ActualCardinality       ?? throw new NullReferenceException("Actual cardinality is null at top of query tree");
            ActualTime = subTree.ActualTime                     ?? throw new NullReferenceException("Actual Time is null at top of query tree");
            QueryTree = subTree;
        }

        public AnalysisResult(string name, decimal? estimatedCost, ulong? estimatedCardinality, ulong? actualCardinality, TimeSpan? actualTime, DataSet inputDataset, string parserName)
            : this(new AnalysisResultQueryTree(name, estimatedCost, estimatedCardinality, actualCardinality, actualTime), inputDataset, parserName)
        { }

        public override string ToString()
        {
            return QueryTree.BuildStringBuilderRec(new StringBuilder(), 0).ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is AnalysisResult result &&
                   Name == result.Name &&
                   EstimatedCost == result.EstimatedCost &&
                   EstimatedCardinality == result.EstimatedCardinality &&
                   ActualCardinality == result.ActualCardinality &&
                   ActualTime.Equals(result.ActualTime) &&
                   EqualityComparer<AnalysisResultQueryTree>.Default.Equals(QueryTree, result.QueryTree) &&
                   EqualityComparer<DataSet>.Default.Equals(InputDataset, result.InputDataset) &&
                   ParserName == result.ParserName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, EstimatedCost, EstimatedCardinality, ActualCardinality, ActualTime, ParserName);
        }
    }
}
