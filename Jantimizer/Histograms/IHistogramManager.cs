using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms.Models;
using Histograms.Models.Histograms;

namespace Histograms
{
    public interface IHistogramManager
    {
        public List<string> Tables { get; }
        public List<string> Attributes { get; }
        public HistogramSet UsedHistograms { get; }

        public Task<List<Task>> AddHistogramsFromDB();
        public void AddHistogram(IHistogram histogram);
        public void ClearHistograms();

        public IHistogram GetHistogram(string table, string attribute);
        public List<IHistogram> GetHistogramsByTable(string table);
        public List<IHistogram> GetHistogramsByAttribute(string attribute);
    }
}
