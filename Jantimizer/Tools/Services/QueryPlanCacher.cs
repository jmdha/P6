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

        public static void AddToCacheIfNotThere(FileInfo queryFile, string testName, ulong actualCardinality) 
            => AddToCacheIfNotThere(File.ReadAllText(queryFile.FullName), testName, actualCardinality);

        public static void AddToCacheIfNotThere(string queryText, string testName, ulong actualCardinality)
        {
            var key = GetKeyFromQuery(queryText, testName);
            if (!ActualCardinalityCache.ContainsKey(key))
                ActualCardinalityCache.Add(key, actualCardinality);
        }

        public static ulong? GetCardinalityOrNull(FileInfo queryFile, string testName)
            => GetCardinalityOrNull(File.ReadAllText(queryFile.FullName), testName);

        public static ulong? GetCardinalityOrNull(string queryText, string testName)
        {
            var key = GetKeyFromQuery(queryText, testName);
            if (ActualCardinalityCache.ContainsKey(key))
                return ActualCardinalityCache[key];
            return null;
        }

        private static string GetKeyFromQuery(string queryText, string testName)
        {
            var res = MD5.HashData(Encoding.ASCII.GetBytes(queryText + testName));
            var key = Encoding.Default.GetString(res);
            return key;
        }
    }
}
