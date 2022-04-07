using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Models;
using DatabaseConnector;
using QueryOptimiser.Models;

namespace QueryOptimiser.Cost.Nodes
{
    public abstract class BaseJoinCost : INodeCost<JoinNode>
    {
        protected int GetMatchBucketIndex(List<IHistogramBucket> buckets, int lowerIndexBound, int upperIndexBound, IComparable value)
        {
            if (upperIndexBound >= lowerIndexBound)
            {
                int mid = lowerIndexBound + (upperIndexBound - lowerIndexBound) / 2;

                if (value.CompareTo(buckets[mid].ValueStart) >= 0 && value.CompareTo(buckets[mid].ValueEnd) <= 0)
                    return mid;

                if (value.CompareTo(buckets[mid].ValueEnd) < 0)
                    return GetMatchBucketIndex(buckets, lowerIndexBound, mid - 1, value);

                return GetMatchBucketIndex(buckets, mid + 1, upperIndexBound, value);
            }

            return -1;
        }

        public CalculationResult CalculateCost(JoinNode node, IHistogramManager histogramManager, BucketLimitation limitation)
        {
            if (node.Relation != null)
            {
                if (node.Relation.Type == RelationType.Type.Predicate && node.Relation.LeafPredicate != null)
                    return CalculateCost(node.Relation.LeafPredicate, histogramManager, limitation);
                else if (node.Relation.Type == RelationType.Type.And || node.Relation.Type == RelationType.Type.Or)
                    return CalculateCost(node.Relation, histogramManager, limitation);
                else
                    throw new ArgumentException("Missing noderelation type " + node.Relation.ToString());
            }
            else
                throw new ArgumentException("node relation is not allowed to be null " + node.ToString());

        }

        private CalculationResult CalculateCost(JoinPredicateRelation nodeRelation, IHistogramManager histogramManager, BucketLimitation limitation)
        {
            JoinPredicateRelation? leftRelation = nodeRelation.LeftRelation;
            JoinPredicateRelation? rightRelation = nodeRelation.RightRelation;

            if (leftRelation != null && rightRelation != null)
            {
                if (nodeRelation.Type == RelationType.Type.And || nodeRelation.Type == RelationType.Type.Or)
                {
                    CalculationResult leftResult = CalculateCost(leftRelation, histogramManager, limitation);
                    CalculationResult rightResult = CalculateCost(rightRelation, histogramManager, limitation);
                    switch (nodeRelation.Type)
                    {
                        case RelationType.Type.And:
                            return new CalculationResult(Math.Min(leftResult.Estimate, rightResult.Estimate), BucketLimitation.MergeOnOverlap(leftResult.BucketLimit, rightResult.BucketLimit));           
                        case RelationType.Type.Or:
                            return new CalculationResult(leftResult.Estimate + rightResult.Estimate, BucketLimitation.Merge(leftResult.BucketLimit, rightResult.BucketLimit));
                        default:
                            throw new Exception($"Can't happen, but compiler is not happy if this doesn't throw an exception(or returns). {nodeRelation.Type.ToString()}");
                    }
                }
                else if (nodeRelation.Type == RelationType.Type.None)
                    throw new ArgumentNullException($"Noderelation type is not set {nodeRelation.ToString()}");
                else
                    throw new NotImplementedException($"The noderelation type of {nodeRelation.Type} is unhandled");
            }
            else if (nodeRelation.LeafPredicate != null)
            {
                return CalculateCost(nodeRelation.LeafPredicate, histogramManager, limitation);
            }
            else
                throw new ArgumentException("Missing noderelation type " + nodeRelation.ToString());
        }

        private CalculationResult CalculateCost(JoinPredicate node, IHistogramManager histogramManager, BucketLimitation limitation)
        {
            List<IHistogramBucket> leftBuckets = new List<IHistogramBucket>();
            List<IHistogramBucket> rightBuckets = new List<IHistogramBucket>();
            if (limitation.PrimaryBuckets.BDictionary.ContainsKey(node.LeftTable.TableName) && limitation.PrimaryBuckets.BDictionary[node.LeftTable.TableName].ContainsKey(node.LeftAttribute))
                leftBuckets = limitation.PrimaryBuckets.BDictionary[node.LeftTable.TableName][node.LeftAttribute];
            else
                leftBuckets = histogramManager.GetHistogram(node.LeftTable.TableName, node.LeftAttribute).Buckets;
            if (limitation.PrimaryBuckets.BDictionary.ContainsKey(node.RightTable.TableName) && limitation.PrimaryBuckets.BDictionary[node.RightTable.TableName].ContainsKey(node.RightAttribute))
                rightBuckets = limitation.PrimaryBuckets.BDictionary[node.RightTable.TableName][node.RightAttribute];
            else
                rightBuckets = histogramManager.GetHistogram(node.RightTable.TableName, node.RightAttribute).Buckets;
            
            int leftStart = 0;
            int leftEnd = leftBuckets.Count - 1;
            int rightStart = 0;
            int rightEnd = rightBuckets.Count - 1;

            if (leftBuckets[0].ValueStart.CompareTo(rightBuckets[0].ValueStart) < 0)
            {
                if (node.ComType != ComparisonType.Type.Less && node.ComType != ComparisonType.Type.EqualOrLess)
                    leftStart = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[0].ValueStart);
            }
            else if (leftBuckets[0].ValueStart.CompareTo(rightBuckets[0].ValueStart) > 0)
            {
                if (node.ComType != ComparisonType.Type.More && node.ComType != ComparisonType.Type.EqualOrMore)
                    rightStart = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[0].ValueStart);
            }

            if (leftBuckets[leftBuckets.Count - 1].ValueEnd.CompareTo(rightBuckets[rightBuckets.Count - 1].ValueEnd) > 0)
            {
                if (node.ComType != ComparisonType.Type.More && node.ComType != ComparisonType.Type.EqualOrMore)
                    leftEnd = GetMatchBucketIndex(leftBuckets, 0, leftBuckets.Count - 1, rightBuckets[rightBuckets.Count - 1].ValueEnd);
            }
            else if (leftBuckets[leftBuckets.Count - 1].ValueEnd.CompareTo(rightBuckets[rightBuckets.Count - 1].ValueEnd) < 0)
            {
                if (node.ComType != ComparisonType.Type.Less && node.ComType != ComparisonType.Type.EqualOrLess)
                    rightEnd = GetMatchBucketIndex(rightBuckets, 0, rightBuckets.Count - 1, leftBuckets[leftBuckets.Count - 1].ValueEnd);
            }

            if (leftStart == -1 || leftEnd == -1 || rightStart == -1 || rightEnd == -1)
                return new CalculationResult(0);

            // This is a tad bit of symptom fixing rather than actual fix
            if (node.ComType == ComparisonType.Type.Less && leftBuckets[leftEnd].ValueEnd.CompareTo(rightBuckets[rightStart].ValueStart) == 0)
                return new CalculationResult(0);
            if (node.ComType == ComparisonType.Type.More && leftBuckets[leftStart].ValueEnd.CompareTo(rightBuckets[rightEnd].ValueEnd) == 0)
                return new CalculationResult(0);

            List<IHistogramBucket> leftMatchBuckets = leftBuckets.GetRange(leftStart, (leftEnd + 1) - leftStart);
            List<IHistogramBucket> rightMatchBuckets = rightBuckets.GetRange(rightStart, (rightEnd + 1) - rightStart);


            BucketDictionary bucketLimitations = new BucketDictionary();
            bucketLimitations.AddBuckets(node.LeftTable.TableName, node.LeftAttribute, leftMatchBuckets);
            bucketLimitations.AddBuckets(node.RightTable.TableName, node.RightAttribute, rightMatchBuckets);


            return new CalculationResult(CalculateCost(node.ComType, leftMatchBuckets, rightMatchBuckets), new BucketLimitation(bucketLimitations));
        }

        protected abstract long CalculateCost(ComparisonType.Type predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets);
    }
}
