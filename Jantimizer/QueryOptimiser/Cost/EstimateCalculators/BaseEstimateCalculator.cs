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
            if (node.Relation == null)
                throw new ArgumentException("node relation is not allowed to be null " + node.ToString());

            BucketMatches bucketMatches = GetBucketMatchesFromRelation(node.Relation, table);
            return new IntermediateTable(bucketMatches.Buckets, bucketMatches.References);
        }

        internal BucketMatches GetBucketMatchesFromRelation(JoinPredicateRelation relation, IntermediateTable table)
        {
            if (relation.Type == RelationType.Type.Predicate && relation.LeafPredicate != null)
                return GetBucketMatchesFromPredicate(relation.LeafPredicate, table);
            else if (relation.LeftRelation != null && relation.RightRelation != null)
            {
                BucketMatches leftMatches = GetBucketMatchesFromRelation(relation.LeftRelation, table);
                BucketMatches rightMatches = GetBucketMatchesFromRelation(relation.RightRelation, table);

                BucketMatches overlap = new BucketMatches();
                if (relation.Type == RelationType.Type.And)
                {
                    overlap.Buckets = BucketHelper.MergeOnOverlap(leftMatches.Buckets, rightMatches.Buckets);
                    overlap.References = GetOverlappingReferences(leftMatches.References, rightMatches.References);
                }
                else if (relation.Type == RelationType.Type.Or)
                {
                    overlap.Buckets.AddRange(leftMatches.Buckets);
                    overlap.Buckets.AddRange(rightMatches.Buckets);
                    overlap.References.AddRange(leftMatches.References);
                    overlap.References.AddRange(rightMatches.References);
                }

                return overlap;
            }
            else
                throw new ArgumentException("Missing noderelation type " + relation.ToString());
        }

        internal BucketMatches GetBucketMatchesFromPredicate(JoinPredicate predicate, IntermediateTable table)
        {
            BucketMatches returnMatches = new BucketMatches();
            PairBucketList bucketPair = GetBucketPair(
                predicate.LeftAttribute,
                predicate.RightAttribute,
                table);
            PairBucketList bounds = BoundsFinder.GetBucketBounds(predicate.ComType, bucketPair.LeftBuckets, bucketPair.RightBuckets);

            if (bounds.LeftBuckets.Count > 0 && bounds.RightBuckets.Count > 0)
            {
                returnMatches.References.Add(predicate.LeftAttribute);
                returnMatches.References.Add(predicate.RightAttribute);
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
