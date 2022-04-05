using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.Services
{
    public class HistogramCache
    {
        public static HistogramCache Instance { get; set; }

        internal static Dictionary<string, object> HistogramCacheDict { get; set; }

        public HistogramCache()
        {
            HistogramCacheDict = new Dictionary<string, object>();
            Instance = this;
        }

        public static void AddToCacheIfNotThere(string table, string attribute, string columnHash, IHistogram histogram)
        {
            var key = GetKeyFromData(table, attribute, columnHash);
            if (!HistogramCacheDict.ContainsKey(key))
                HistogramCacheDict.Add(key, histogram.Clone());
        }

        public static IHistogram? GetHistogramOrNull(string table, string attribute, string columnHash)
        {
            var key = GetKeyFromData(table, attribute, columnHash);
            if (HistogramCacheDict.ContainsKey(key))
                if (HistogramCacheDict[key] is IHistogram histo)
                    return histo.Clone() as IHistogram;
            return null;
        }

        private static string GetKeyFromData(string table, string attribute, string columnHash)
        {
            var res = MD5.HashData(Encoding.ASCII.GetBytes(table.ToLower() + attribute.ToLower() + columnHash.ToLower()));
            var key = Encoding.Default.GetString(res);
            return key;
        }
    }
}
