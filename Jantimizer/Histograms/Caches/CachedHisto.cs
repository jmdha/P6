using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Histograms.Caches
{
    internal class CachedHisto
    {
        public List<CachedBucket> Buckets { get; }
        public string TableName { get; }
        public string AttributeName { get; }
        public int Depth { get; }
        public string TypeName { get; }
        public string Hash { get; }

        [JsonConstructorAttribute]
        public CachedHisto(List<CachedBucket> buckets, string tableName, string attributeName, int depth, string typeName, string hash)
        {
            Buckets = buckets;
            TableName = tableName;
            AttributeName = attributeName;
            Depth = depth;
            TypeName = typeName;
            Hash = hash;
        }

        public CachedHisto(IHistogram histogram, string hash)
        {
            throw new NotImplementedException("Unknown histogram type");
        }

        public CachedHisto(IDepthHistogram histogram, string hash)
        {
            if (histogram is not HistogramEquiDepth || histogram is not HistogramEquiDepthVariance)
                throw new NotImplementedException("Unknown histogram type");

            List<CachedBucket> newBuckets = new List<CachedBucket>();
            foreach (var bucket in histogram.Buckets)
                newBuckets.Add(new CachedBucket(bucket));
            Buckets = newBuckets;
            TableName = histogram.TableName;
            AttributeName = histogram.AttributeName;
            Depth = histogram.Depth;
            Hash = hash;
            TypeName = "HistogramEquiDepthVariance";
        }
    }
}
