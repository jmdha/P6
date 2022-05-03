using Histograms;
using Histograms.Models;
using Histograms.Models.Histograms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Stubs
{
    internal class TestHistogramManager : IHistogramManager
    {
        private Dictionary<TableAttribute, IHistogram> Histograms { get; set; } = new Dictionary<TableAttribute, IHistogram>();
        public List<string> Tables => throw new NotImplementedException();

        public List<string> Attributes => throw new NotImplementedException();

        public HistogramSet UsedHistograms => throw new NotImplementedException();

        public string RunnerName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ExperimentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddHistogram(IHistogram histogram)
        {
            Histograms.Add(histogram.TableAttribute, histogram);
        }

        public Task<List<Task>> AddHistogramsFromDB()
        {
            throw new NotImplementedException();
        }

        public void ClearHistograms()
        {
            throw new NotImplementedException();
        }

        public ulong GetAbstractDatabaseSizeBytes()
        {
            throw new NotImplementedException();
        }

        public ulong GetAbstractStorageBytes()
        {
            throw new NotImplementedException();
        }

        public IHistogram GetHistogram(string table, string attribute)
        {
            return Histograms[new TableAttribute(table, attribute)];
        }
    }
}
