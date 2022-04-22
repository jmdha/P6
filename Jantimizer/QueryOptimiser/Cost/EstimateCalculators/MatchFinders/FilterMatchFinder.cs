using Histograms.Models;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Cost.EstimateCalculators.MatchFinders
{
    public class FilterMatchFinder : BaseMatchFinder<IFilterEstimate>
    {
        public FilterMatchFinder(IFilterEstimate estimator) : base(estimator)
        {
        }

        internal List<IntermediateBucket> GetMatches(FilterNode node, List<IHistogramBucket> buckets)
        {
            switch (node.ComType)
            {
                case ComparisonType.Type.Equal:
                    return GetEqualityMatches(node, buckets);
                case ComparisonType.Type.Less:
                case ComparisonType.Type.More:
                case ComparisonType.Type.EqualOrMore:
                case ComparisonType.Type.EqualOrLess:
                    return GetInEqualityMatches(node, buckets);
                default:
                    throw new ArgumentException($"Invalid comparison type {node.ToString()}");
            }
        }

        internal List<IntermediateBucket> GetEqualityMatches(FilterNode node, List<IHistogramBucket> buckets)
        {
            List<IntermediateBucket> intermediateBuckets = new List<IntermediateBucket>();
            bool matches = false;
            for (int i = 0; i < buckets.Count; i++)
            {
                if (DoesOverlap(node.Constant, buckets[i]))
                {
                    matches = true;
                    intermediateBuckets.Add(MakeNewIntermediateBucket(MatchType.Overlap, node.ComType, node.Constant, new TableAttribute(node.TableReference.Alias, node.AttributeName), buckets[i]));
                }
                else if (matches)
                    break;
            }
            return intermediateBuckets;
        }

        internal List<IntermediateBucket> GetInEqualityMatches(FilterNode node, List<IHistogramBucket> buckets)
        {
            List<IntermediateBucket> intermediateBuckets = new List<IntermediateBucket>();
            for (int i = 0; i < buckets.Count; i++)
            {
                MatchType type = DoesMatch(node.ComType, node.Constant, buckets[i]);
                if (type == MatchType.Match || type == MatchType.Overlap)
                    intermediateBuckets.Add(MakeNewIntermediateBucket(type, node.ComType, node.Constant, new TableAttribute(node.TableReference.Alias, node.AttributeName), buckets[i]));
            }
            return intermediateBuckets;
        }

        internal IntermediateBucket MakeNewIntermediateBucket(MatchType matchType, ComparisonType.Type comparisonType, IComparable constant, TableAttribute tableAttribute, IHistogramBucket bucket)
        {
            var newBucket = new IntermediateBucket();
            long count;
            if (matchType == MatchType.Match)
                count = bucket.Count;
            else if (matchType == MatchType.Overlap)
                count = Estimator.GetBucketEstimate(comparisonType, constant, bucket);
            else
                throw new ArgumentException($"Invalid matchtype {matchType}");

            newBucket.AddBucketIfNotThere(
                tableAttribute,
                new BucketEstimate(bucket, count)
                );
            return newBucket;
        }
    }
}
