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
using QueryOptimiser.Models;

namespace QueryOptimiser.Cost.CostCalculators
{
    public interface ICostCalculator
    {
        public IHistogramManager HistogramManager { get; }

        public CalculationResult CalculateCost(INode node);
    }
}
