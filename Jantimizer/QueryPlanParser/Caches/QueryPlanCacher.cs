using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;

namespace QueryPlanParser.Caches
{
    public class QueryPlanCacher : ICacherService<ulong?>
    {
        public static QueryPlanCacher? Instance { get; set; }

        internal Dictionary<string, ulong> ActualCardinalityCache { get; set; }

        public QueryPlanCacher()
        {
            ActualCardinalityCache= new Dictionary<string, ulong>();
            Instance = this;
        }

        public void AddToCacheIfNotThere(string hashValue, ulong? value)
        {
            if (!ActualCardinalityCache.ContainsKey(hashValue))
                if (value != null)
                    ActualCardinalityCache.Add(hashValue, (ulong)value);
        }

        public void AddToCacheIfNotThere(string[] hashValues, ulong? value)
        {
            string hashValue = GetCacheKey(hashValues);
            AddToCacheIfNotThere(hashValue, value);
        }

        public ulong? GetValueOrNull(string hashValue)
        {
            if (ActualCardinalityCache.ContainsKey(hashValue))
                return ActualCardinalityCache[hashValue];
            return null;
        }

        public ulong? GetValueOrNull(string[] hashValues)
        {
            string hashValue = GetCacheKey(hashValues);
            return GetValueOrNull(hashValue);
        }

        public string GetCacheKey(string[] values)
        {
            var concat = string.Join("",values);
            var res = MD5.HashData(Encoding.ASCII.GetBytes(concat.ToLower()));
            var key = Encoding.Default.GetString(res);
            return key;
        }
    }
}
