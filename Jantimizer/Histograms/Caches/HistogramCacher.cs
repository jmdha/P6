using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;

namespace Histograms.Caches
{
    public class HistogramCacher : ICacherService<IHistogram>
    {
        public static HistogramCacher? Instance { get; set; }

        internal Dictionary<string, IHistogram> HistogramCacheDict { get; set; }

        public HistogramCacher()
        {
            HistogramCacheDict = new Dictionary<string, IHistogram>();
            Instance = this;
        }

        public void AddToCacheIfNotThere(string hashValue, IHistogram value)
        {
            if (!HistogramCacheDict.ContainsKey(hashValue))
                HistogramCacheDict.Add(hashValue, value);
        }

        public void AddToCacheIfNotThere(string[] hashValues, IHistogram value)
        {
            string hashValue = GetCacheKey(hashValues);
            AddToCacheIfNotThere(hashValue, value);
        }

        public IHistogram GetValueOrNull(string hashValue)
        {
            if (HistogramCacheDict.ContainsKey(hashValue))
                return HistogramCacheDict[hashValue];
            return null;
        }

        public IHistogram GetValueOrNull(string[] hashValues)
        {
            string hashValue = GetCacheKey(hashValues);
            return GetValueOrNull(hashValue);
        }

        public string GetCacheKey(string[] values)
        {
            var concat = string.Join("", values);
            var res = MD5.HashData(Encoding.ASCII.GetBytes(concat.ToLower()));
            var key = Encoding.Default.GetString(res);
            return key;
        }
    }
}
