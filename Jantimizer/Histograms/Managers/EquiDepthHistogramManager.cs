using Histograms.Caches;
using Histograms.DataGatherers;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;
using Tools.Models;

namespace Histograms.Managers
{
    public class EquiDepthHistogramManager : BaseHistogramManager, IDepthHistogramManager
    {
        public int Depth { get; }

        public EquiDepthHistogramManager(IDataGatherer dataGatherer, int depth) : base(dataGatherer)
        {
            Depth = depth;
        }

        protected override async Task<IHistogram> CreateHistogramForAttribute(string tableName, string attributeName)
        {
            IDepthHistogram histogram = new HistogramEquiDepth(tableName, attributeName, Depth);
            histogram.GenerateHistogramFromSortedGroups(await DataGatherer.GetSortedGroupsFromDb(tableName, attributeName));
            return histogram;
        }

        internal async Task<IHistogram?> GetHistogramFromCacheOrNull(string tableName, string attributeName)
        {
            if (HistogramCacher.Instance == null)
                return null;
            string hash = await DataGatherer.GetTableAttributeColumnHash(tableName, attributeName);
            var retVal = HistogramCacher.Instance.GetValueOrNull(new string[] { tableName, attributeName, hash, Depth.ToString() });
            return retVal;
        }

        protected override async Task<IHistogram?> GetCachedHistogramOrNull(string tableName, string attributeName)
        {
            var cacheHisto = await GetHistogramFromCacheOrNull(tableName, attributeName);

            if (cacheHisto != null && cacheHisto is HistogramEquiDepth)
                return cacheHisto;

            return null;
        }

        protected override async Task CacheHistogram(string tableName, string attributeName, IHistogram histogram)
        {
            string columnHash = await DataGatherer.GetTableAttributeColumnHash(tableName, attributeName);
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.AddToCacheIfNotThere(new string[] { tableName, attributeName, columnHash, Depth.ToString() }, histogram);
        }
    }
}
