using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms
{
    public class HistogramManager<T> : IHistogramManager<T> where T : IHistogram
    {
        public List<IHistogram> Histograms { get; }
        public List<string> Tables => Histograms.Select(x => x.TableName).ToList();
        public List<string> Attributes(string table) => Histograms.Where(x => x.TableName == table).Select(x => x.AttributeName).ToList();

        public HistogramManager()
        {
            Histograms = new List<IHistogram>();
        }

        public async Task AddHistogram(string setupQuery)
        {
            if (setupQuery.ToUpper().StartsWith("CREATE TABLE "))
            {

            }
        }

        public async Task AddHistogram(FileInfo setupQueryFile)
        {
            foreach (string line in File.ReadAllLines(setupQueryFile.FullName))
                await AddHistogram(line);
        }

        public void AddHistogram(IHistogram histogram)
        {
            Histograms.Add(histogram);
        }
    }
}
