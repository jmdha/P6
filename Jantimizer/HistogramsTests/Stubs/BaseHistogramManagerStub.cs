using Histograms.DataGatherers;
using Histograms.Managers;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Stubs
{
    internal class BaseHistogramManagerStub : BaseHistogramManager
    {
        public BaseHistogramManagerStub() : base(new DataGathererStub()) { }

        public BaseHistogramManagerStub(IDataGatherer dataGatherer) : base(dataGatherer) { }

        protected override Task CacheHistogram(string tableName, string attributeName, IHistogram histogram)
        {
            throw new NotImplementedException();
        }

        protected override Task<IHistogram> CreateHistogramForAttribute(string attributeName, string tableName)
        {
            throw new NotImplementedException();
        }

        protected override Task<IHistogram?> GetCachedHistogramOrNull(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }
    }
}
