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
        protected int GetMatchBucketIndex(IHistogram gram, int lowerIndexBound, int upperIndexBound, IComparable value)
        {
            if (upperIndexBound >= lowerIndexBound)
            {
                int mid = lowerIndexBound + (upperIndexBound - lowerIndexBound) / 2;

                if (value.CompareTo(gram.Buckets[mid].ValueStart) >= 0 && value.CompareTo(gram.Buckets[mid].ValueEnd) <= 0)
                    return mid;

                if (value.CompareTo(gram.Buckets[mid].ValueEnd) < 0)
                    return GetMatchBucketIndex(gram, lowerIndexBound, mid - 1, value);

                return GetMatchBucketIndex(gram, mid + 1, upperIndexBound, value);
            }

            return -1;
        }

        public CalculationResult CalculateCost(JoinNode node, IHistogramManager histogramManager)
        {
            if (node.Relation != null)
            {
                if (node.Relation.Type == RelationType.Type.Predicate && node.Relation.LeafPredicate != null)
                    return CalculateCost(node.Relation.LeafPredicate, histogramManager);
                else if (node.Relation.Type == RelationType.Type.And || node.Relation.Type == RelationType.Type.Or)
                    return CalculateCost(node.Relation, histogramManager);
                else
                    throw new ArgumentException("Missing noderelation type " + node.Relation.ToString());
            }
            else
                throw new ArgumentException("node relation is not allowed to be null " + node.ToString());

        }

        private CalculationResult CalculateCost(JoinPredicateRelation nodeRelation, IHistogramManager histogramManager)
        {
            JoinPredicateRelation? leftRelation = nodeRelation.LeftRelation;
            JoinPredicateRelation? rightRelation = nodeRelation.RightRelation;

            if (leftRelation != null && rightRelation != null)
            {
                if (nodeRelation.Type == RelationType.Type.And || nodeRelation.Type == RelationType.Type.Or)
                {
                    CalculationResult leftResult = CalculateCost(leftRelation, histogramManager);
                    CalculationResult rightResult = CalculateCost(rightRelation, histogramManager);
                    switch (nodeRelation.Type)
                    {
                        case RelationType.Type.And:
                            return new CalculationResult(Math.Min(leftResult.Estimate, rightResult.Estimate));
                        //throw new NotImplementedException("IMPLEMENT DICTIONARY ADDITION OF BOTH LIMITATIONS OF BUCKETS");
                        // if table and attribute is the same return only those who are in both dictionaries
                        // Else return the sum of all dictioanry, i.e. all limitations
                        case RelationType.Type.Or:
                            return new CalculationResult(leftResult.Estimate + rightResult.Estimate, leftResult.BucketLimit, rightResult.BucketLimit);
                        default:
                            throw new Exception($"Can't happen, but compiler is not happy if this doesn't throw an exception. {nodeRelation.Type.ToString()}");
                    }
                }
                else if (nodeRelation.Type == RelationType.Type.None)
                    throw new ArgumentNullException($"Noderelation type is not set {nodeRelation.ToString()}");
                else
                    throw new NotImplementedException($"The noderelation type of {nodeRelation.Type} is unhandled");
            }
            else if (nodeRelation.LeafPredicate != null)
            {
                return CalculateCost(nodeRelation.LeafPredicate, histogramManager);
            }
            else
                throw new ArgumentException("Missing noderelation type " + nodeRelation.ToString());
        }

        private CalculationResult CalculateCost(JoinPredicate node, IHistogramManager histogramManager)
        {
            IHistogram leftGram = histogramManager.GetHistogram(node.LeftTable.TableName, node.LeftAttribute);
            IHistogram rightGram = histogramManager.GetHistogram(node.RightTable.TableName, node.RightAttribute);
            int leftStart = 0;
            int leftEnd = leftGram.Buckets.Count - 1;
            int rightStart = 0;
            int rightEnd = rightGram.Buckets.Count - 1;

            if (leftGram.Buckets[0].ValueStart.CompareTo(rightGram.Buckets[0].ValueStart) < 0)
            {
                if (node.ComType != ComparisonType.Type.Less && node.ComType != ComparisonType.Type.EqualOrLess)
                    leftStart = GetMatchBucketIndex(leftGram, 0, leftGram.Buckets.Count - 1, rightGram.Buckets[0].ValueStart);
            }
            else if (leftGram.Buckets[0].ValueStart.CompareTo(rightGram.Buckets[0].ValueStart) > 0)
            {
                if (node.ComType != ComparisonType.Type.More && node.ComType != ComparisonType.Type.EqualOrMore)
                    rightStart = GetMatchBucketIndex(rightGram, 0, rightGram.Buckets.Count - 1, leftGram.Buckets[0].ValueStart);
            }

            if (leftGram.Buckets[leftGram.Buckets.Count - 1].ValueEnd.CompareTo(rightGram.Buckets[rightGram.Buckets.Count - 1].ValueEnd) > 0)
            {
                if (node.ComType != ComparisonType.Type.More && node.ComType != ComparisonType.Type.EqualOrMore)
                    leftEnd = GetMatchBucketIndex(leftGram, 0, leftGram.Buckets.Count - 1, rightGram.Buckets[rightGram.Buckets.Count - 1].ValueEnd);
            }
            else if (leftGram.Buckets[leftGram.Buckets.Count - 1].ValueEnd.CompareTo(rightGram.Buckets[rightGram.Buckets.Count - 1].ValueEnd) < 0)
            {
                if (node.ComType != ComparisonType.Type.Less && node.ComType != ComparisonType.Type.EqualOrLess)
                    rightEnd = GetMatchBucketIndex(rightGram, 0, rightGram.Buckets.Count - 1, leftGram.Buckets[leftGram.Buckets.Count - 1].ValueEnd);
            }

            if (leftStart == -1 || leftEnd == -1 || rightStart == -1 || rightEnd == -1)
                return new CalculationResult(0);

            // This is a tad bit of symptom fixing rather than actual fix
            if (node.ComType == ComparisonType.Type.Less && leftGram.Buckets[leftEnd].ValueEnd.CompareTo(rightGram.Buckets[rightStart].ValueStart) == 0)
                return new CalculationResult(0);
            if (node.ComType == ComparisonType.Type.More && leftGram.Buckets[leftStart].ValueEnd.CompareTo(rightGram.Buckets[rightEnd].ValueEnd) == 0)
                return new CalculationResult(0);

            List<IHistogramBucket> leftBuckets = leftGram.Buckets.GetRange(leftStart, (leftEnd + 1) - leftStart);
            List<IHistogramBucket> rightBuckets = leftGram.Buckets.GetRange(rightStart, (rightEnd + 1) - rightStart);


            BucketDictionary bucketLimitations = new BucketDictionary();
            bucketLimitations.AddBuckets(node.LeftTable.TableName, node.LeftAttribute, leftBuckets);
            bucketLimitations.AddBuckets(node.RightTable.TableName, node.RightAttribute, rightBuckets);


            return new CalculationResult(CalculateCost(node.ComType, leftBuckets, rightBuckets), new BucketLimitation(bucketLimitations));
        }

        protected abstract long CalculateCost(ComparisonType.Type predicate, List<IHistogramBucket> leftBuckets, List<IHistogramBucket> rightBuckets);
    }
}
