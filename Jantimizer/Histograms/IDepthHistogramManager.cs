using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms.Models;

namespace Histograms
{
    public delegate int DepthCalculator(long uniqueValueCount, long totalValueCount);
    public interface IDepthHistogramManager : IHistogramManager
    {
        public DepthCalculator GetDepth { get; }
    }
}
