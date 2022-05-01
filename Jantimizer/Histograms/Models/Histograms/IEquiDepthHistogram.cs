using System.Data;

namespace Histograms.Models
{
    public interface IDepthHistogram : IHistogram
    {
        public DepthCalculator GetDepth { get; }
    }
}
