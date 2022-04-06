using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Caches
{
    public interface ICacherService<T>
    {
        public bool UseCacheFile { get; set; }
        public FileInfo CacheFile { get; set; }
        public void AddToCacheIfNotThere(string hashValue, T value);
        public void AddToCacheIfNotThere(string[] hashValues, T value);
        public T? GetValueOrNull(string hashValue);
        public T? GetValueOrNull(string[] hashValues);
        public string GetCacheKey(string[] hashValues);
        public List<CacheItem> GetAllCacheItems();

        public void ClearCache(bool deleteFile = false);
        public void LoadCacheFromFile();
        public void SaveCacheToFile();
    }
}
