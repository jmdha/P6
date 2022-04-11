using Histograms.Caches;
using Histograms.DataGatherers;
using Histograms.HistogramSelectors;
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
    public class EquiDepthVarianceHistogramManager : EquiDepthHistogramManager, IDepthHistogramManager
    {

        public EquiDepthVarianceHistogramManager(IDataGatherer dataGatherer, int depth) : base(dataGatherer, depth)
        { }

        protected override async Task<IHistogram> CreateHistogramForAttribute(string tableName, string attributeName)
        {
            IDepthHistogramSelector<IDepthHistogram> selector = new DepthHistogramSelector();
            IDepthHistogram histogram = selector.GetHistogramDepthOfTypeOrAlt(
                new HistogramEquiDepthVariance(tableName, attributeName, Depth),
                await DataGatherer.GetAttributeType(tableName, attributeName),
                tableName,
                attributeName,
                Depth);
            histogram.GenerateHistogramFromSortedGroups(await DataGatherer.GetSortedGroupsFromDb(tableName, attributeName));
            return histogram;
        }

        protected override async Task CacheHistogram(string tableName, string attributeName, IHistogram histogram)
        {
            string columnHash = await DataGatherer.GetTableAttributeColumnHash(tableName, attributeName);
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.AddToCacheIfNotThere(new string[] { tableName, attributeName, columnHash, Depth.ToString(), typeof(HistogramEquiDepthVariance).Name }, histogram);
        }
    }
}
