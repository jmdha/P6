using DatabaseConnector;
using DatabaseConnector.Connectors;
using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System.Data;
using System.Text;
using Tools.Models;
using Histograms.Models;
using Histograms.DataGatherers;

namespace Histograms.Managers
{
    public class MySQLEquiDepthVarianceHistogramManager : BaseEquiDepthHistogramManager
    {
        public MySQLEquiDepthVarianceHistogramManager(ConnectionProperties connectionProperties, int depth) : this(new MySqlDataGatherer(connectionProperties), depth)
        { }
        public MySQLEquiDepthVarianceHistogramManager(MySqlDataGatherer dataGatherer, int depth) : base(dataGatherer, depth)
        { }
    }
}
