using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tools.Caches;

namespace Histograms.Caches
{
    public class HistogramCacher : BaseCacherService<IHistogram>
    {
        public override FileInfo CacheFile { get; set; }
        public static HistogramCacher? Instance { get; set; }

        internal Dictionary<string, IHistogram> HistogramCacheDict { get; set; }

        public HistogramCacher(string cacheFilePath)
        {
            HistogramCacheDict = new Dictionary<string, IHistogram>();
            CacheFile = new FileInfo(cacheFilePath);
            if (CacheFile.Exists)
                LoadCacheFromFile();
            Instance = this;
        }

        public override void AddToCacheIfNotThere(string hashValue, IHistogram value)
        {
            if (!HistogramCacheDict.ContainsKey(hashValue))
            {
                if (value != null)
                {
                    HistogramCacheDict.Add(hashValue, value);
                    SaveCacheToFile();
                }
            }
        }

        public override IHistogram? GetValueOrNull(string hashValue)
        {
            if (HistogramCacheDict.ContainsKey(hashValue))
                return HistogramCacheDict[hashValue];
            return null;
        }

        public override void ClearCache()
        {
            HistogramCacheDict.Clear();
        }

        public override void LoadCacheFromFile()
        {
            if (CacheFile.Exists)
            {
                ClearCache();
                var list = JsonSerializer.Deserialize<List<CachedHisto>>(File.ReadAllText(CacheFile.FullName));
                if (list != null)
                {
                    foreach (var value in list)
                    {
                        switch (value.TypeName)
                        {
                            case "HistogramEquiDepth":
                                var newHistoEqui = new HistogramEquiDepth(value.TableName, value.AttributeName, value.Depth);
                                foreach (var bucket in value.Buckets)
                                    newHistoEqui.Buckets.Add(ConvertBucket(bucket));
                                HistogramCacheDict.Add(value.Hash, newHistoEqui);
                                break;
                            case "HistogramEquiDepthVariance":
                                var newHistoVariance = new HistogramEquiDepthVariance(value.TableName, value.AttributeName, value.Depth);
                                foreach (var bucket in value.Buckets)
                                    newHistoVariance.Buckets.Add(ConvertBucket(bucket));
                                HistogramCacheDict.Add(value.Hash, newHistoVariance);
                                break;
                        }
                    }
                }
            }
        }

        private IHistogramBucket ConvertBucket(CachedBucket bucket)
        {
            switch (bucket.TypeName)
            {
                case "HistogramBucket": return new HistogramBucket(bucket.ValueStart, bucket.ValueEnd, bucket.Count);
                case "HistogramBucketVariance": return new HistogramBucketVariance(bucket.ValueStart, bucket.ValueEnd, bucket.Count, bucket.Variance, bucket.Mean);
            }
            throw new InvalidCastException("Unknown bucket type!");
        }

        public override void SaveCacheToFile()
        {
            List<CachedHisto> histograms = new List<CachedHisto>();
            foreach(var key in HistogramCacheDict.Keys)
            {
                var addHisto = HistogramCacheDict[key];
                if (addHisto is IDepthHistogram depthHisto)
                    histograms.Add(new CachedHisto(depthHisto, key));
                if (addHisto is IHistogram defaultHisto)
                    histograms.Add(new CachedHisto(defaultHisto, key));
            }
            string jsonText = JsonSerializer.Serialize(histograms);
            if (CacheFile.Exists)
                CacheFile.Delete();

            using (FileStream fs = CacheFile.Create())
            {
                byte[] info = new UTF8Encoding(true).GetBytes(jsonText);
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
