using Histograms.Managers;
using Histograms;
using QueryOptimiser.QueryGenerators;
using QueryParser;
using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;
using QueryOptimiser.Cost.CostCalculators;
using QueryOptimiser.Cost.Nodes;

namespace QueryOptimiser
{
    public interface IQueryOptimiser<HistogramType, ConnectorType>
        where HistogramType : IHistogram
        where ConnectorType : IDbConnector
    {
        public IParserManager ParserManager { get; set; }
        public IQueryGenerator QueryGenerator { get; set; }
        public IHistogramManager<HistogramType, ConnectorType> HistogramManager { get; set; }
        public ICostCalculator<HistogramType, ConnectorType> CostCalculator { get; set; }

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public string OptimiseQuery(string query);

        /// <summary>
        /// Reorders a querys join order according to the cost of each join operation
        /// </summary>
        /// <returns></returns>
        public string OptimiseQuery(List<INode> nodes);

        /// <summary>
        /// Calculates worst case cost of each join operation
        /// </summary>
        /// <returns></returns>
        public List<ValuedNode> ValueQuery(List<INode> nodes);
    }
}
