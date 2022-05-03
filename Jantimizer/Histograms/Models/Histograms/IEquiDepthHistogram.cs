using Histograms.DepthCalculators;
using System.Data;

namespace Histograms.Models
{
    public interface IDepthHistogram : IHistogram
    {
        public IDepthCalculator DepthCalculator { get; }
    }
}
