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


        protected override Task<IHistogram> CreateHistogramForAttribute(string attributeName, string tableName)
        {
            throw new NotImplementedException();
        }

        protected override string[] GetCacheHashString(string tableName, string attributeName, string columnHash)
        {
            throw new NotImplementedException();
        }
    }
}
