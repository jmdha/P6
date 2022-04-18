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
        public int Depth { get; }

        public MinDepthHistogramManager(IDataGatherer dataGatherer, int depth) : base(dataGatherer)
        {
            Depth = depth;
        }

        protected override async Task<IHistogram> CreateHistogramForAttribute(string tableName, string attributeName)
        {
            IDepthHistogramSelector<IDepthHistogram> selector = new DepthHistogramSelector();
            IDepthHistogram histogram = selector.GetHistogramDepthOfTypeOrAlt(
                new HistogramMinDepth(tableName, attributeName, Depth),
                await DataGatherer.GetAttributeType(tableName, attributeName),
                tableName,
                attributeName,
                Depth);
            histogram.GenerateHistogramFromSortedGroups(await DataGatherer.GetSortedGroupsFromDb(tableName, attributeName));
            return histogram;
        }

        protected override string[] GetCacheHashString(string tableName, string attributeName, string columnHash) =>
            new string[] { tableName, attributeName, columnHash, Depth.ToString(), typeof(HistogramMinDepth).Name };
    }
}
