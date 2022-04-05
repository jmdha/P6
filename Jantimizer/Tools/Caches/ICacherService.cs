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
        public static ICacherService<T>? Instance { get; set; }
        public void AddToCacheIfNotThere(string hashValue, T value);
        public void AddToCacheIfNotThere(string[] hashValues, T value);
        public T? GetValueOrNull(string hashValue);
        public T? GetValueOrNull(string[] hashValues);
        public string GetCacheKey(string[] hashValues);
    }
}
