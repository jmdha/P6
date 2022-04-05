using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Caches
{
    public class HistogramCache : ICacheService<object>
    {
        public static HistogramCache? Instance { get; set; }

        internal Dictionary<string, object> HistogramCacheDict { get; set; }

        public HistogramCache()
        {
            HistogramCacheDict = new Dictionary<string, object>();
            Instance = this;
        }

        public void AddToCacheIfNotThere(string hashValue, object value)
        {
            if (!HistogramCacheDict.ContainsKey(hashValue))
                HistogramCacheDict.Add(hashValue, value);
        }

        public void AddToCacheIfNotThere(string[] hashValues, object value)
        {
            string hashValue = GetCacheKey(hashValues);
            AddToCacheIfNotThere(hashValue, value);
        }

        public object GetValueOrNull(string hashValue)
        {
            if (HistogramCacheDict.ContainsKey(hashValue))
                return HistogramCacheDict[hashValue];
            return null;
        }

        public object GetValueOrNull(string[] hashValues)
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
