using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using Histograms;
using Histograms.Managers;
using DatabaseConnector;

namespace QueryOptimiser.Cost.CostCalculators
{
    public interface ICostCalculator<HistogramType, DbConnectorType>
        where HistogramType : IHistogram
        where DbConnectorType : IDbConnector
    {
        public IHistogramManager<HistogramType, DbConnectorType> HistogramManager { get; set; }

        public int CalculateCost(INode node);
    }
}
