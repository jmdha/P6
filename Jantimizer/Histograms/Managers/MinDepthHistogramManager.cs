using Histograms.Caches;
using Histograms.DataGatherers;
using Histograms.DepthCalculators;
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
        public IDepthCalculator DepthCalculator { get; }

        public MinDepthHistogramManager(IDataGatherer dataGatherer, IDepthCalculator depthCalculator) : base(dataGatherer)
        {
            DepthCalculator = depthCalculator;
        }

        protected override async Task<IHistogram> CreateHistogramForAttribute(string tableName, string attributeName)
        {
            IDepthHistogramSelector<IDepthHistogram> selector = new DepthHistogramSelector();
            IDepthHistogram histogram = selector.GetHistogramDepthOfTypeOrAlt(
                new HistogramMinDepth(tableName, attributeName, DepthCalculator),
                await DataGatherer.GetAttributeType(tableName, attributeName),
                tableName,
                attributeName,
                DepthCalculator);
            histogram.GenerateSegmentationsFromSortedGroups(await DataGatherer.GetSortedGroupsFromDb(tableName, attributeName));
            return histogram;
        }

        protected override string[] GetCacheHashString(string tableName, string attributeName, string columnHash) =>
            new string[] { tableName, attributeName, columnHash, DepthCalculator.GetHashCode().ToString(), RunnerName, ExperimentName, typeof(HistogramMinDepth).Name };
    }
}
