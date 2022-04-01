using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Services
{
    public class QueryPlanCacher
    {
        public static QueryPlanCacher Instance { get; set; }

        internal static Dictionary<string, ulong> ActualCardinalityCache { get; set; }

        public QueryPlanCacher()
        {
            ActualCardinalityCache= new Dictionary<string, ulong>();
            Instance = this;
        }

        public static void AddToCacheIfNotThere(string query, ulong actualCardinality)
        {
            var key = GetKeyFromQuery(query);
            if (!ActualCardinalityCache.ContainsKey(key))
                ActualCardinalityCache.Add(key, actualCardinality);
        }

        public static ulong? GetCardinalityOrNull(string query)
        {
            var key = GetKeyFromQuery(query);
            if (ActualCardinalityCache.ContainsKey(key))
                return ActualCardinalityCache[key];
            return null;
        }

        private static string GetKeyFromQuery(string query)
        {
            var res = MD5.HashData(Encoding.ASCII.GetBytes(query));
            var key = Encoding.Default.GetString(res);
            return key;
        }
    }
}
