using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Models
{
    internal class BucketLimitation
    {
        internal BucketDictionary PrimaryBuckets { get; }

        internal BucketLimitation()
        {
            PrimaryBuckets = new BucketDictionary();
        }

        internal BucketLimitation(BucketDictionary primaryBuckets)
        {
            PrimaryBuckets = primaryBuckets;
        }

        // Creates a limitation with both possibilities from the parameters
        static internal BucketLimitation Merge(BucketLimitation? bucketLimitation1, BucketLimitation? bucketLimitation2)
        {
            BucketLimitation merged = new BucketLimitation();
            if (bucketLimitation1 != null)
                merged.PrimaryBuckets.AddBuckets(bucketLimitation1.PrimaryBuckets);
            if (bucketLimitation2 != null)
                merged.PrimaryBuckets.AddBuckets(bucketLimitation2.PrimaryBuckets);
            return merged;
        }

        // Creates a limitation with both possibilities from the paramters
        // Except for cases where both contain the same table and attribute combination
        //      Then it only takes those which are in both parameters
        static internal BucketLimitation MergeOnOverlap(BucketLimitation? bucketLimitation1, BucketLimitation? bucketLimitation2)
        {
            BucketLimitation merged = new BucketLimitation();
            if (bucketLimitation1 == null || bucketLimitation2 == null)
            {
                if (bucketLimitation1 != null)
                    merged.PrimaryBuckets.AddBuckets(bucketLimitation1.PrimaryBuckets);
                else if (bucketLimitation2 != null)
                    merged.PrimaryBuckets.AddBuckets(bucketLimitation2.PrimaryBuckets);
            }
            else
            {
                List<IHistogramBucket> uncheckedBuckets = new List<IHistogramBucket>();
                foreach (var tableLimit in bucketLimitation2.PrimaryBuckets.BDictionary)
                    foreach (var attributeLimit in tableLimit.Value)
                        foreach (IHistogramBucket bucket in attributeLimit.Value)
                            uncheckedBuckets.Add(bucket);
                foreach (var tableLimit in bucketLimitation1.PrimaryBuckets.BDictionary)
                {
                    // If both dictionaries contain the same table
                    if (bucketLimitation2.PrimaryBuckets.BDictionary.ContainsKey(tableLimit.Key))
                    {
                        foreach (var attributeLimit in tableLimit.Value)
                        {
                            // If those dictionaries contain the same attribute
                            if (bucketLimitation2.PrimaryBuckets.BDictionary[tableLimit.Key].ContainsKey(attributeLimit.Key))
                            {
                                List<IHistogramBucket> matchingBuckets = new List<IHistogramBucket>();
                                // Find buckets which are in both
                                foreach (IHistogramBucket bucket in attributeLimit.Value)
                                {
                                    if (bucketLimitation2.PrimaryBuckets.BDictionary[tableLimit.Key][attributeLimit.Key].Contains(bucket))
                                    {
                                        matchingBuckets.Add(bucket);
                                        uncheckedBuckets.Remove(bucket);
                                    } 
                                }
                                merged.PrimaryBuckets.AddBuckets(tableLimit.Key, attributeLimit.Key, matchingBuckets);
                            }
                        }
                    }
                }
                foreach (var tableLimit in bucketLimitation2.PrimaryBuckets.BDictionary)
                    foreach (var attributeLimit in tableLimit.Value)
                        foreach (IHistogramBucket bucket in attributeLimit.Value)
                            if (!uncheckedBuckets.Contains(bucket))
                                merged.PrimaryBuckets.AddBucket(tableLimit.Key, attributeLimit.Key, bucket);
            }
            return merged;
        }
    }
}
