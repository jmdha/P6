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
        public int ValueStart { get; }
        public int ValueEnd { get; }
        public long Count { get; }
        public int Variance { get; }
        public int Mean { get; }
        public string TypeName { get; }

        [JsonConstructorAttribute]
        public CachedBucket(int valueStart, int valueEnd, long count, int variance, int mean, string typeName)
        {
            ValueStart = valueStart;
            ValueEnd = valueEnd;
            Count = count;
            Variance = variance;
            Mean = mean;
            TypeName = typeName;
        }

        public CachedBucket(HistogramBucket bucket)
        {
            ValueStart = (int)bucket.ValueStart;
            ValueEnd = (int)bucket.ValueEnd;
            Count = bucket.Count;
            TypeName = nameof(HistogramBucket);
        }

        public CachedBucket(HistogramBucketVariance bucket)
        {
            ValueStart = (int)bucket.ValueStart;
            ValueEnd = (int)bucket.ValueEnd;
            Count = bucket.Count;
            Variance = bucket.Variance;
            Mean = bucket.Mean;
            TypeName = nameof(HistogramBucketVariance);
        }

        public CachedBucket(IHistogramBucket bucket)
        {
            throw new NotImplementedException("Unknown bucket type");
        }
    }
}
