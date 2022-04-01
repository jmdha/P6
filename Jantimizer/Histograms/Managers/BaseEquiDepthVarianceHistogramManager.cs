using Histograms.DataGatherers;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace Histograms.Managers
{
    public abstract class BaseEquiDepthVarianceHistogramManager : BaseEquiDepthHistogramManager, IDepthHistogramManager
    {

        protected BaseEquiDepthVarianceHistogramManager(IDataGatherer dataGatherer, int depth) : base(dataGatherer, depth)
        { }

        protected override async Task AddHistogramForAttribute(string attributeName, string tableName)
        {
            IDepthHistogram newHistogram = new HistogramEquiDepthVariance(tableName, attributeName, Depth);
            newHistogram.GenerateHistogramFromSortedGroups(await DataGatherer.GetSortedGroupsFromDb(tableName, attributeName));
            await Task.Delay(1);

            AddHistogram(newHistogram);
        }
    }
}
