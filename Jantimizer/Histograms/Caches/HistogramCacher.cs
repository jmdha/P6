using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                //var obj = JsonSerializer.Deserialize<Dictionary<string, IHistogram>>(File.ReadAllText(CacheFile.FullName));
                //if (obj != null)
                //    HistogramCacheDict = obj;
            }
        }

        public override void SaveCacheToFile()
        {
            string jsonText = JsonSerializer.Serialize(HistogramCacheDict);
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
