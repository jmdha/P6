using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms
{
    public interface IHistogramManager<T> where T : IHistogram
    {
        public List<IHistogram> Histograms { get; }
        public List<string> Tables => Histograms.Select(x => x.TableName).ToList();
        public List<string> Attributes(string table) => Histograms.Where(x => x.TableName == table).Select(x => x.AttributeName).ToList();
        public Task AddHistogram(string setupQuery);
        public Task AddHistogram(FileInfo setupQueryFile);
        public void AddHistogram(IHistogram histogram);
    }
}
