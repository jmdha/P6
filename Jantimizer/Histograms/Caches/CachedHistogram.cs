using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Histograms.Caches
{
    public class CachedHistogram
    {
        public List<CachedBucket> Buckets { get; }
        public string TableName { get; }
        public string AttributeName { get; }
        public int Depth { get; }
        public string TypeName { get; }
        public string Hash { get; }
        public Guid HistogramId { get; }

        [JsonConstructorAttribute]
        public CachedHistogram(Guid histogramId, List<CachedBucket> buckets, string tableName, string attributeName, int depth, string typeName, string hash)
        {
            HistogramId = histogramId;
            Buckets = buckets;
            TableName = tableName;
            AttributeName = attributeName;
            Depth = depth;
            TypeName = typeName;
            Hash = hash;
        }

        public CachedHistogram(HistogramEquiDepth histogram, string hash)
        {
            List<CachedBucket> newBuckets = new List<CachedBucket>();
            foreach (var bucket in histogram.Buckets)
                newBuckets.Add(new CachedBucket((dynamic)bucket));
            HistogramId = histogram.HistogramId;
            Buckets = newBuckets;
            TableName = histogram.TableName;
            AttributeName = histogram.AttributeName;
            Depth = histogram.Depth;
            Hash = hash;
            TypeName = nameof(HistogramEquiDepth);
        }

        public CachedHistogram(HistogramMinDepth histogram, string hash)
        {
            List<CachedBucket> newBuckets = new List<CachedBucket>();
            foreach (var bucket in histogram.Buckets)
                newBuckets.Add(new CachedBucket((dynamic)bucket));
            HistogramId = histogram.HistogramId;
            Buckets = newBuckets;
            TableName = histogram.TableName;
            AttributeName = histogram.AttributeName;
            Depth = histogram.Depth;
            Hash = hash;
            TypeName = nameof(HistogramMinDepth);
        }

        public CachedHistogram(HistogramEquiDepthVariance histogram, string hash)
        {
            List<CachedBucket> newBuckets = new List<CachedBucket>();
            foreach (var bucket in histogram.Buckets)
                newBuckets.Add(new CachedBucket((dynamic)bucket));
            HistogramId = histogram.HistogramId;
            Buckets = newBuckets;
            TableName = histogram.TableName;
            AttributeName = histogram.AttributeName;
            Depth = histogram.Depth;
            Hash = hash;
            TypeName = nameof(HistogramEquiDepthVariance);
        }

        public CachedHistogram(IHistogram histogram, string hash)
        {
            throw new NotImplementedException("Unknown histogram type for caching!");
        }
    }
}
