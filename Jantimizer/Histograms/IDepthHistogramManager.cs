using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms.Models;
using Histograms.DepthCalculators;

namespace Histograms
{
    public interface IDepthHistogramManager : IHistogramManager
    {
        public IDepthCalculator DepthCalculator { get; }
    }
}
