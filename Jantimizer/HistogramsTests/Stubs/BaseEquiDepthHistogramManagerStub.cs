using Histograms.DataGatherers;
using Histograms.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Stubs
{
    internal class BaseEquiDepthHistogramManagerStub : BaseEquiDepthHistogramManager
    {
        public BaseEquiDepthHistogramManagerStub(int depth) : this(new DataGathererStub(), depth) { }

        public BaseEquiDepthHistogramManagerStub(IDataGatherer dataGatherer, int depth) : base(dataGatherer, depth) { }

        protected override Task AddHistogramForAttribute(string attributeName, string tableName)
        {
            throw new NotImplementedException();
        }

    }
}
