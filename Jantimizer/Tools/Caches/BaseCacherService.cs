using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Caches
{
    public abstract class BaseCacherService<T> : ICacherService<T>
    {
        public abstract FileInfo CacheFile { get; set; }
        public abstract void AddToCacheIfNotThere(string hashValue, T value);
        public void AddToCacheIfNotThere(string[] hashValues, T value) => AddToCacheIfNotThere(GetCacheKey(hashValues), value);
        public abstract T? GetValueOrNull(string hashValue);
        public T? GetValueOrNull(string[] hashValues) => GetValueOrNull(GetCacheKey(hashValues));
        public string GetCacheKey(string[] hashValues)
        {
            var concat = string.Join("", hashValues);
            var res = MD5.HashData(Encoding.ASCII.GetBytes(concat.ToLower()));
            var key = Encoding.Default.GetString(res);
            return key;
        }

        public abstract void ClearCache();
        public abstract void LoadCacheFromFile();
        public abstract void SaveCacheToFile();
    }
}
