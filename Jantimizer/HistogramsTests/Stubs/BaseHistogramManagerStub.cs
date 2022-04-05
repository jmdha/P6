using Histograms.DataGatherers;
using Histograms.Managers;
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

        protected override Task AddHistogramForAttribute(string attributeName, string tableName)
        {
            throw new NotImplementedException();
        }

    }
}
