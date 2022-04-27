using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.Dictionaries;
using Tools.Models.JsonModels;

[assembly:InternalsVisibleTo("QueryOptimiserTest")]

namespace QueryOptimiser.Models
{
    public class IntermediateBucket
    {
        internal DualDictionary<TableAttributeDictRef, BucketEstimate> Buckets { get; } = new DualDictionary<TableAttributeDictRef, BucketEstimate>();

        public IntermediateBucket() { }

        public IntermediateBucket(IntermediateBucket bucket1, IntermediateBucket bucket2)
        {
            AddBucketsIfNotThere(bucket1);
            AddBucketsIfNotThere(bucket2);
        }

        internal long GetEstimateOfAllBuckets()
        {
            long count = 1;
            foreach (var tableAttribute in Buckets.Keys)
                count *= Buckets[tableAttribute].Estimate;
            return count;
        }

        internal void AddBucketsIfNotThere(IntermediateBucket bucket)
        {
            foreach (var tableAttribute in bucket.Buckets.Keys)
                AddBucketIfNotThere(tableAttribute, bucket.Buckets[tableAttribute]);
        }

        internal void AddBucketIfNotThere(TableAttribute tableAttribute, BucketEstimate bucket)
        {
            var key = new TableAttributeDictRef(tableAttribute.Table.TableName, tableAttribute.Attribute);
            AddBucketIfNotThere(key, bucket);
        }

        internal void AddBucketIfNotThere(TableAttributeDictRef key, BucketEstimate bucket)
        {
            if (!Buckets.DoesContain(key))
                Buckets.Add(key, bucket);
        }

        public override bool Equals(object? obj)
        {
            if (obj is IntermediateBucket bucket)
            {
                foreach (var tableAttribute in Buckets.Keys)
                    if (!bucket.Buckets.DoesContain(tableAttribute))
                        return false;
                return true;
            }
            return false;   
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var tableAttribute in Buckets.Keys)
                hash += Buckets[tableAttribute].GetHashCode();
            return hash;
        }
    }
}
