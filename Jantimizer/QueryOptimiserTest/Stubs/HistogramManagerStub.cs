using Histograms;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest.Stubs
{
    public class HistogramManagerStub : IHistogramManager
    {
        public List<string> Tables => throw new NotImplementedException();

        public List<string> Attributes => throw new NotImplementedException();

        public void AddHistogram(IHistogram histogram)
        {
            throw new NotImplementedException();
        }

        public Task<List<Task>> AddHistogramsFromDB()
        {
            throw new NotImplementedException();
        }

        public void ClearHistograms()
        {
            throw new NotImplementedException();
        }

        public IHistogram GetHistogram(string table, string attribute)
        {
            throw new NotImplementedException();
        }

        public List<IHistogram> GetHistogramsByAttribute(string attribute)
        {
            throw new NotImplementedException();
        }

        public List<IHistogram> GetHistogramsByTable(string table)
        {
            throw new NotImplementedException();
        }
    }
}
