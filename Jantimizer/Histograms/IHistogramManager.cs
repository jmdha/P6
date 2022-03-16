using DatabaseConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms
{
    public interface IHistogramManager<HistogramType, ConnectorType> 
        where HistogramType : IHistogram 
        where ConnectorType : IDbConnector
    {
        public ConnectorType DbConnector { get; }
        public List<IHistogram> Histograms { get; }
        public List<string> Tables => Histograms.Select(x => x.TableName).ToList();
        public List<string> Attributes(string table) => Histograms.Where(x => x.TableName == table).Select(x => x.AttributeName).ToList();

        public Task AddHistograms(string setupQuery);
        public Task AddHistograms(FileInfo setupQueryFile);
        public void AddHistogram(IHistogram histogram);

        public IHistogram GetHistogram(string table, string attribute);
        public List<IHistogram> GetHistogramsByTable(string table);
        public List<IHistogram> GetHistogramsByAttribute(string attribute);
    }
}
