using Histograms;
using Histograms.Models;
using Histograms.Models.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest.Stubs
{
    public class HistogramManagerStub : IHistogramManager
    {
        public List<IHistogram> TestStorage { get; set; } = new List<IHistogram>();

        public List<string> Tables => throw new NotImplementedException();

        public List<string> Attributes => throw new NotImplementedException();

        public HistogramSet UsedHistograms => throw new NotImplementedException();

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
            foreach (IHistogram histogram in TestStorage)
                if (histogram.AttributeName == attribute)
                    if (histogram.TableName == table)
                        return histogram;
            throw new KeyNotFoundException();
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
