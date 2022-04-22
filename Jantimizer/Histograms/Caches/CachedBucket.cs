using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Histograms.Caches
{
    public class CachedBucket
    {
        public Guid BucketId { get; }
        public string ValueStart { get; }
        public string ValueEnd { get; }
        public long Count { get; }
        public double Variance { get; }
        public double StandardDeviation { get; }
        public double Range { get; }
        public double Mean { get; }
        public string TypeName { get; }
        public string ValueType { get; }

        [JsonConstructorAttribute]
        public CachedBucket(Guid bucketId, string valueStart, string valueEnd, long count, double variance, double mean, double standardDeviation, double range, string typeName, string valueType)
        {
            BucketId = bucketId;
            ValueStart = valueStart;
            ValueEnd = valueEnd;
            Count = count;
            Variance = variance;
            Mean = mean;
            StandardDeviation = standardDeviation;
            Range = range;
            TypeName = typeName;
            ValueType = valueType;
        }

        public CachedBucket(HistogramBucket bucket)
        {
            BucketId = bucket.BucketId;
            ValueStart = $"{bucket.ValueStart}";
            ValueEnd = $"{bucket.ValueEnd}";
            Count = bucket.Count;
            ValueType = bucket.ValueStart.GetType().ToString();
            TypeName = nameof(HistogramBucket);
        }

        public CachedBucket(HistogramBucketVariance bucket)
        {
            BucketId = bucket.BucketId;
            ValueStart = $"{bucket.ValueStart}";
            ValueEnd = $"{bucket.ValueEnd}";
            Count = bucket.Count;
            Variance = bucket.Variance;
            Mean = bucket.Mean;
            StandardDeviation = bucket.StandardDeviation;
            Range = bucket.Range;
            ValueType = bucket.ValueStart.GetType().ToString();
            TypeName = nameof(HistogramBucketVariance);
        }

        public CachedBucket(IHistogramBucket bucket)
        {
            throw new NotImplementedException("Unknown bucket type");
        }
    }
}
