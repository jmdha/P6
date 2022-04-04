using DatabaseConnector;
using DatabaseConnector.Connectors;
using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using Tools.Models;
using Histograms.Models;
using Histograms.DataGatherers;

namespace Histograms.Managers
{
    public class PostgresEquiDepthVarianceHistogramManager : BaseEquiDepthVarianceHistogramManager
    {
        public PostgresEquiDepthVarianceHistogramManager(ConnectionProperties connectionProperties, int depth) : this(new PostgresDataGatherer(connectionProperties), depth)
        { }
        public PostgresEquiDepthVarianceHistogramManager(PostgresDataGatherer dataGatherer, int depth) : base(dataGatherer, depth)
        { }
    }
}
