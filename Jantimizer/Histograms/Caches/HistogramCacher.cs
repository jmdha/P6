using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;

namespace Histograms.Caches
{
    public class HistogramCacher : BaseCacherService<IHistogram>
    {
        public static HistogramCacher? Instance { get; set; }

        internal Dictionary<string, IHistogram> HistogramCacheDict { get; set; }

        public HistogramCacher()
        {
            HistogramCacheDict = new Dictionary<string, IHistogram>();
            Instance = this;
        }

        public override void AddToCacheIfNotThere(string hashValue, IHistogram value)
        {
            if (!HistogramCacheDict.ContainsKey(hashValue))
                HistogramCacheDict.Add(hashValue, value);
        }

        public override IHistogram? GetValueOrNull(string hashValue)
        {
            if (HistogramCacheDict.ContainsKey(hashValue))
                return HistogramCacheDict[hashValue];
            return null;
        }
    }
}
