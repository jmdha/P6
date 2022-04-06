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
        public string ValueStart { get; }
        public string ValueEnd { get; }
        public long Count { get; }
        public int Variance { get; }
        public int Mean { get; }
        public string TypeName { get; }
        public string ValueType { get; }

        [JsonConstructorAttribute]
        public CachedBucket(string valueStart, string valueEnd, long count, int variance, int mean, string typeName, string valueType)
        {
            ValueStart = valueStart;
            ValueEnd = valueEnd;
            Count = count;
            Variance = variance;
            Mean = mean;
            TypeName = typeName;
            ValueType = valueType;
        }

        public CachedBucket(HistogramBucket bucket)
        {
            ValueStart = $"{bucket.ValueStart}";
            ValueEnd = $"{bucket.ValueEnd}";
            Count = bucket.Count;
            ValueType = bucket.ValueStart.GetType().ToString();
            TypeName = nameof(HistogramBucket);
        }

        public CachedBucket(HistogramBucketVariance bucket)
        {
            ValueStart = $"{bucket.ValueStart}";
            ValueEnd = $"{bucket.ValueEnd}";
            Count = bucket.Count;
            Variance = bucket.Variance;
            Mean = bucket.Mean;
            ValueType = bucket.ValueStart.GetType().ToString();
            TypeName = nameof(HistogramBucketVariance);
        }

        public CachedBucket(IHistogramBucket bucket)
        {
            throw new NotImplementedException("Unknown bucket type");
        }
    }
}
