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
        public FileInfo CacheFile { get; set; }
        public static ICacherService<T>? Instance { get; set; }
        public void AddToCacheIfNotThere(string hashValue, T value);
        public void AddToCacheIfNotThere(string[] hashValues, T value);
        public T? GetValueOrNull(string hashValue);
        public T? GetValueOrNull(string[] hashValues);
        public string GetCacheKey(string[] hashValues);

        public void ClearCache();
        public void LoadCacheFromFile();
        public void SaveCacheToFile();
    }
}
