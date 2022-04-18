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
            if (CacheFile.Exists && UseCacheFile)
                LoadCacheFromFile();
            Instance = this;
        }

        public HistogramCacher()
        {
            HistogramCacheDict = new Dictionary<string, IHistogram>();
            CacheFile = new FileInfo("none");
            UseCacheFile = false;
            Instance = this;
        }

        public override void AddToCacheIfNotThere(string hashValue, IHistogram value)
        {
            if (!HistogramCacheDict.ContainsKey(hashValue))
            {
                if (value != null)
                {
                    HistogramCacheDict.Add(hashValue, value);
                    if (UseCacheFile)
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

        public override void ClearCache(bool deleteFile = false)
        {
            if (CacheFile.Exists && deleteFile)
                CacheFile.Delete();
            HistogramCacheDict.Clear();
        }

        public override void LoadCacheFromFile()
        {
            if (CacheFile.Exists)
            {
                ClearCache();
                var list = JsonSerializer.Deserialize<List<CachedHistogram>>(File.ReadAllText(CacheFile.FullName));
                if (list != null)
                {
                    foreach (var value in list)
                    {
                        switch (value.TypeName)
                        {
                            case "HistogramEquiDepth":
                                HistogramCacheDict.Add(value.Hash, new HistogramEquiDepth(value));
                                break;
                            case "HistogramEquiDepthVariance":
                                HistogramCacheDict.Add(value.Hash, new HistogramEquiDepthVariance(value));
                                break;
                        }
                    }
                }
            }
        }

        public override void SaveCacheToFile()
        {
            List<CachedHistogram> histograms = new List<CachedHistogram>();
            foreach(var key in HistogramCacheDict.Keys)
                histograms.Add(new CachedHistogram((dynamic)HistogramCacheDict[key], key));

            string jsonText = JsonSerializer.Serialize(histograms);
            if (CacheFile.Exists)
                CacheFile.Delete();

            using (FileStream fs = CacheFile.Create())
            {
                byte[] info = new UTF8Encoding(true).GetBytes(jsonText);
                fs.Write(info, 0, info.Length);
            }
        }

        public override List<CacheItem> GetAllCacheItems()
        {
            var returnList = new List<CacheItem>();
            foreach (var key in HistogramCacheDict.Keys)
            {
                var content = HistogramCacheDict[key].ToString();
                if (content != null)
                    returnList.Add(new CacheItem(key, content, nameof(HistogramCacher)));
            }
            return returnList;
        }
    }
}
