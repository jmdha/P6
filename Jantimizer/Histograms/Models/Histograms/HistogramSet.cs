using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.Models.Histograms
{
    public class HistogramSet : ICloneable
    {
        public List<IHistogram> Histograms { get; set; }

        public HistogramSet()
        {
            Histograms = new List<IHistogram>();
        }

        public HistogramSet(List<IHistogram> histograms)
        {
            Histograms = histograms;
        }

        public void AddHistograms(List<IHistogram> histograms)
        {
            foreach (IHistogram histogram in histograms)
            {
                var clone = histogram.Clone() as IHistogram;
                if (clone != null)
                    Histograms.Add(clone);
            }
        }

        public void AddHistogram(IHistogram histogram)
        {
            var clone = histogram.Clone() as IHistogram;
            if (clone != null)
                Histograms.Add(clone);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (IHistogram histogram in Histograms)
                hash += histogram.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(IHistogram histogram in Histograms)
                sb.Append(histogram.ToString());
            return sb.ToString();
        }

        public object Clone()
        {
            var newList = new List<IHistogram>();
            foreach (var node in Histograms)
                if (node.Clone() is IHistogram clone)
                    newList.Add(clone);
            return new HistogramSet(newList);
        }
    }
}
