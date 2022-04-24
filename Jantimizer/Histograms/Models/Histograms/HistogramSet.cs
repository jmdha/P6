using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.Models.Histograms
{
    public class HistogramSet : ICloneable
    {
        public Dictionary<int, IHistogram> Histograms { get; set; }

        public HistogramSet()
        {
            Histograms = new Dictionary<int, IHistogram>();
        }

        public HistogramSet(Dictionary<int, IHistogram> histograms)
        {
            Histograms = histograms;
        }

        public void AddHistograms(List<IHistogram> histograms)
        {
            foreach (IHistogram histogram in histograms)
            {
                var clone = histogram.Clone() as IHistogram;
                if (clone != null) {
                    int hash = clone.GetHashCode();
                    if (!Histograms.ContainsKey(hash))
                        Histograms.Add(hash, clone);
                }
            }
        }

        public void AddHistogram(IHistogram histogram)
        {
            var clone = histogram.Clone() as IHistogram;
            if (clone != null)
            {
                int hash = clone.GetHashCode();
                if (!Histograms.ContainsKey(hash))
                    Histograms.Add(hash, clone);
            }
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (IHistogram histogram in Histograms.Values)
                hash += histogram.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(IHistogram histogram in Histograms.Values)
                sb.Append(histogram.ToString());
            return sb.ToString();
        }

        public object Clone()
        {
            var newList = new Dictionary<int, IHistogram>();
            foreach (var key in Histograms.Keys)
                if (Histograms[key].Clone() is IHistogram clone)
                    newList.Add(key, clone);
            return new HistogramSet(newList);
        }
    }
}
