using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    public class CalculationResult
    {
        public long Estimate { get; }
        internal BucketLimitation? BucketLimit { get; set; }

        internal CalculationResult(long estimate)
        {
            Estimate = estimate;
        }

        internal CalculationResult(long estimate, BucketLimitation bucketLimit)
        {
            Estimate = estimate;
            BucketLimit = bucketLimit;
        }

        static internal CalculationResult CreateAndCalculationResult(long estimate, BucketLimitation? bucketLimit1, BucketLimitation? bucketLimit2)
        {
            CalculationResult result = new CalculationResult(estimate);
            if (bucketLimit1 == null && bucketLimit2 == null)
                return result;
            else if (bucketLimit1 == null)
                result.BucketLimit = bucketLimit2;
            else if (bucketLimit2 == null)
                result.BucketLimit = bucketLimit1;
            else
            {
                BucketLimitation bucketLimit = new BucketLimitation();
                List<IHistogramBucket> uncheckedBuckets = new List<IHistogramBucket>();
                foreach (var tableLimit in bucketLimit2.PrimaryBuckets.BDictionary)
                    foreach (var attributeLimit in tableLimit.Value)
                        foreach (IHistogramBucket bucket in attributeLimit.Value)
                            uncheckedBuckets.Add(bucket);
                foreach (var tableLimit in bucketLimit1.PrimaryBuckets.BDictionary)
                {
                    // If both dictionaries contain the same table
                    if (bucketLimit2.PrimaryBuckets.BDictionary.ContainsKey(tableLimit.Key))
                    {
                        foreach (var attributeLimit in tableLimit.Value)
                        {
                            // If those dictionaries contain the same attribute
                            if (bucketLimit2.PrimaryBuckets.BDictionary[tableLimit.Key].ContainsKey(attributeLimit.Key))
                            {
                                List<IHistogramBucket> matchingBuckets = new List<IHistogramBucket>();
                                // Find buckets which are in both
                                foreach (IHistogramBucket bucket in attributeLimit.Value)
                                {
                                    if (bucketLimit2.PrimaryBuckets.BDictionary[tableLimit.Key][attributeLimit.Key].Contains(bucket))
                                    {
                                        matchingBuckets.Add(bucket);
                                        uncheckedBuckets.Remove(bucket);
                                    }
                                }
                                bucketLimit.PrimaryBuckets.AddBuckets(tableLimit.Key, attributeLimit.Key, matchingBuckets);
                            }
                        }
                    }
                }
                foreach (var tableLimit in bucketLimit2.PrimaryBuckets.BDictionary)
                    foreach (var attributeLimit in tableLimit.Value)
                        foreach (IHistogramBucket bucket in attributeLimit.Value)
                            if (!uncheckedBuckets.Contains(bucket))
                                bucketLimit.PrimaryBuckets.AddBucket(tableLimit.Key, attributeLimit.Key, bucket);
                result.BucketLimit = bucketLimit;
            }
            return result;
        }

        static internal CalculationResult CreateOrCalculationResult(long estimate, BucketLimitation? bucketLimit1, BucketLimitation? bucketLimit2)
        {
            CalculationResult result = new CalculationResult(estimate);
            if (bucketLimit1 != null && bucketLimit2 != null)
                result.BucketLimit = new BucketLimitation(bucketLimit1.PrimaryBuckets, bucketLimit2.PrimaryBuckets);
            else if (bucketLimit1 != null)
                result.BucketLimit = new BucketLimitation(bucketLimit1.PrimaryBuckets);
            else if (bucketLimit2 != null)
                result.BucketLimit = new BucketLimitation(bucketLimit2.PrimaryBuckets);
            else
                result.BucketLimit = null;
            return result;
        }
    }
}
