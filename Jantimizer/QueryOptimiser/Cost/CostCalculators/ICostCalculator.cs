using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Managers;
using Histograms.Models;
using DatabaseConnector;

namespace QueryOptimiser.Cost.CostCalculators
{
    public interface ICostCalculator
    {
        public IHistogramManager HistogramManager { get; set; }

        public long CalculateCost(INode node);
    }
}
