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
    public class PostgresEquiDepthHistogramManager : BaseEquiDepthHistogramManager
    {
        public PostgresEquiDepthHistogramManager(ConnectionProperties connectionProperties, int depth) : this(new PostgresDataGatherer(connectionProperties), depth)
        { }
        public PostgresEquiDepthHistogramManager(PostgresDataGatherer dataGatherer, int depth) : base(dataGatherer, depth)
        { }
    }
}
