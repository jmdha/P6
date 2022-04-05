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
    public class EquiDepthVarianceHistogramManager : EquiDepthHistogramManager, IDepthHistogramManager
    {

        public EquiDepthVarianceHistogramManager(IDataGatherer dataGatherer, int depth) : base(dataGatherer, depth)
        { }

        protected override async Task<IHistogram> CreateHistogramForAttribute(string tableName, string attributeName)
        {
            IHistogram histogram = new HistogramEquiDepthVariance(tableName, attributeName, Depth);
            histogram.GenerateHistogramFromSortedGroups(await DataGatherer.GetSortedGroupsFromDb(tableName, attributeName));
            return histogram;
        }

        protected override async Task<IHistogram?> GetCachedHistogramOrNull(string tableName, string attributeName)
        {
            var cacheHisto = await DataGatherer.GetHistogramFromCacheOrNull(tableName, attributeName);

            if(cacheHisto != null && cacheHisto is HistogramEquiDepthVariance)
                return cacheHisto;

            return null;
        }
    }
}
