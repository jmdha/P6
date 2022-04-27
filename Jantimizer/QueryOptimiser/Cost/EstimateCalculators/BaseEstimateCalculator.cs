using Histograms;
using Histograms.Models;
using QueryOptimiser.Cost.EstimateCalculators.MatchFinders;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Helpers;
using QueryOptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Helpers;
using Tools.Models.JsonModels;

namespace QueryOptimiser.Cost.EstimateCalculators
{
    public abstract class BaseEstimateCalculator : IEstimateCalculator
    {
        public IHistogramManager HistogramManager { get; set; }
        public abstract JoinMatchFinder JoinMatcher { get; set; }
        public abstract FilterMatchFinder FilterMatcher { get; set; }

        public BaseEstimateCalculator(IHistogramManager manager)
        {
            HistogramManager = manager;
        }

        public IntermediateTable EstimateIntermediateTable(INode node, IntermediateTable intermediateTable)
        {
            if (node is JoinNode joinNode)
                return EstimateJoinTable(joinNode, intermediateTable);
            else if (node is FilterNode filterNode)
                return EstimateFilterTable(filterNode, intermediateTable);
            else
                throw new ArgumentException("Non handled node type " + node.ToString());
        }

        #region Filter
        internal IntermediateTable EstimateFilterTable(FilterNode node, IntermediateTable intermediateTable)
        {
            List<IHistogramBucket> buckets = GetBuckets(node.FilterAttribute, intermediateTable);
            List<IntermediateBucket> intermediateBuckets = FilterMatcher.GetMatches(node, buckets);
            return new IntermediateTable(intermediateBuckets, new List<TableAttribute>() { node.FilterAttribute });
        }
        #endregion

        #region Join
        internal IntermediateTable EstimateJoinTable(JoinNode node, IntermediateTable table)
        {
            BucketMatches bucketMatches = GetBucketMatchesFromRelation(node.Predicates, table);
            return new IntermediateTable(bucketMatches.Buckets, bucketMatches.References);
        }

        internal BucketMatches GetBucketMatchesFromRelation(List<JoinPredicate> predicates, IntermediateTable table)
        {
            if (predicates.Count > 1)
            {
                BucketMatches leftMatches = GetBucketMatchesFromRelation(new List<JoinPredicate>() { predicates[0] }, table);
                BucketMatches rightMatches = GetBucketMatchesFromRelation(new List<JoinPredicate>(predicates.GetRange(1, predicates.Count - 1)), table);

                BucketMatches overlap = new BucketMatches();
                overlap.Buckets = BucketHelper.MergeOnOverlap(leftMatches.Buckets, rightMatches.Buckets);
                overlap.References = GetOverlappingReferences(leftMatches.References, rightMatches.References);
                return overlap;
            }
            else if (predicates.Count == 1)
            {
                return GetBucketMatchesFromPredicate(predicates[0], table);
            }
            throw new ArgumentException("Could not calculate bucket match!");
        }

        internal BucketMatches GetBucketMatchesFromPredicate(JoinPredicate predicate, IntermediateTable table)
        {
            BucketMatches returnMatches = new BucketMatches();
            PairBucketList bucketPair = GetBucketPair(
                predicate.LeftAttribute.Attribute!,
                predicate.RightAttribute.Attribute!,
                table);
            PairBucketList bounds = BoundsFinder.GetBucketBounds(predicate.GetComType(), bucketPair.LeftBuckets, bucketPair.RightBuckets);

            if (bounds.LeftBuckets.Count > 0 && bounds.RightBuckets.Count > 0)
            {
                returnMatches.References.Add(predicate.LeftAttribute.Attribute!);
                returnMatches.References.Add(predicate.RightAttribute.Attribute!);
            }
            else
                return returnMatches;

            returnMatches.Buckets = JoinMatcher.GetMatches(predicate, bounds.LeftBuckets, bounds.RightBuckets);

            return returnMatches;
        }
        #endregion

        internal List<IHistogramBucket> GetBuckets(TableAttribute tableRefNode, IntermediateTable table)
        {
            if (table.References.Contains(tableRefNode))
                return table.GetBuckets(tableRefNode);
            else
                return GetHistogram(tableRefNode).Buckets;
        }

        internal PairBucketList GetBucketPair(TableAttribute leftTableRefNode, TableAttribute rightTableRefNode, IntermediateTable table)
        {
            List<IHistogramBucket> leftBuckets = GetBuckets(leftTableRefNode, table);
            List<IHistogramBucket> rightBuckets = GetBuckets(rightTableRefNode, table);
            return new PairBucketList(leftBuckets, rightBuckets);
        }

        internal List<TableAttribute> GetOverlappingReferences(List<TableAttribute> leftReferences, List<TableAttribute> rightReferences)
        {
            List<TableAttribute> overlappingReferences = new List<TableAttribute>();
            foreach (var leftReference in leftReferences)
                foreach (var rightReference in rightReferences)
                    if (leftReference.Equals(rightReference))
                        overlappingReferences.Add(leftReference);
            return overlappingReferences;
        }

        internal IHistogram GetHistogram(TableAttribute tableAttribute)
        {
            return HistogramManager.GetHistogram(tableAttribute.Table.TableName, tableAttribute.Attribute);
        }
    }
}
