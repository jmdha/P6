using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Histograms.Models;

namespace Histograms
{
    public interface IDepthHistogramManager : IHistogramManager
    {
        public int Depth { get; }
    }
}
