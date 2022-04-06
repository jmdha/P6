using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;

namespace ToolsTests.Stubs
{
    internal class TestBaseCacherService : BaseCacherService<int?>
    {
        public override FileInfo CacheFile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Dictionary<string, int> _cache = new Dictionary<string, int>();

        public TestBaseCacherService()
        {
        }

        public override void AddToCacheIfNotThere(string hashValue, int? value)
        {
            if (!_cache.ContainsKey(hashValue))
                if (value != null)
                    _cache.Add(hashValue, (int)value);
        }

        public override int? GetValueOrNull(string hashValue)
        {
            if (_cache.ContainsKey(hashValue))
                return _cache[hashValue];
            return null;
        }

        public override void ClearCache(bool deleteFile = false)
        {
            throw new NotImplementedException();
        }

        public override List<CacheItem> GetAllCacheItems()
        {
            throw new NotImplementedException();
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
