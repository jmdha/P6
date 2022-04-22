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

        public override void LoadCacheFromFile()
        {
            throw new NotImplementedException();
        }

        public override void SaveCacheToFile()
        {
            throw new NotImplementedException();
        }
    }
}
