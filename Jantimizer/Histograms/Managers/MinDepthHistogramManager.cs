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
    public class MinDepthHistogramManager : BaseHistogramManager, IDepthHistogramManager
    {
        public DepthCalculator GetDepth { get; }

        public MinDepthHistogramManager(IDataGatherer dataGatherer, DepthCalculator getDepth) : base(dataGatherer)
        {
            GetDepth = getDepth;
        }

        protected override async Task<IHistogram> CreateHistogramForAttribute(string tableName, string attributeName)
        {
            IDepthHistogramSelector<IDepthHistogram> selector = new DepthHistogramSelector();
            IDepthHistogram histogram = selector.GetHistogramDepthOfTypeOrAlt(
                new HistogramMinDepth(tableName, attributeName, GetDepth),
                await DataGatherer.GetAttributeType(tableName, attributeName),
                tableName,
                attributeName,
                GetDepth);
            histogram.GenerateSegmentationsFromSortedGroups(await DataGatherer.GetSortedGroupsFromDb(tableName, attributeName));
            return histogram;
        }

        protected override string[] GetCacheHashString(string tableName, string attributeName, string columnHash) =>
            new string[] { tableName, attributeName, columnHash, GetDepth.GetHashCode().ToString(), typeof(HistogramMinDepth).Name };
    }
}
