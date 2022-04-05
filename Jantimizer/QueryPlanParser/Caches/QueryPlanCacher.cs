using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;

namespace QueryPlanParser.Caches
{
    public class QueryPlanCacher : BaseCacherService<ulong?>
    {
        public static QueryPlanCacher? Instance { get; set; }

        internal Dictionary<string, ulong> ActualCardinalityCache { get; set; }

        public QueryPlanCacher()
        {
            ActualCardinalityCache= new Dictionary<string, ulong>();
            Instance = this;
        }

        public override void AddToCacheIfNotThere(string hashValue, ulong? value)
        {
            if (!ActualCardinalityCache.ContainsKey(hashValue))
                if (value != null)
                    ActualCardinalityCache.Add(hashValue, (ulong)value);
        }

        public override ulong? GetValueOrNull(string hashValue)
        {
            if (ActualCardinalityCache.ContainsKey(hashValue))
                return ActualCardinalityCache[hashValue];
            return null;
        }
    }
}
