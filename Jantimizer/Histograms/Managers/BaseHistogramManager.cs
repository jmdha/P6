using DatabaseConnector;
using Histograms.Caches;
using Histograms.DataGatherers;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace Histograms.Managers
{
    public abstract class BaseHistogramManager : IHistogramManager
    {
        private Dictionary<string, Dictionary<string, IHistogram>> Histograms { get; set; } = new Dictionary<string, Dictionary<string, IHistogram>>();


        public List<string> Tables => Histograms.Select(x => x.Key).ToList();
        public List<string> Attributes
        {
            get
            {
                var returnList = new List<string>();
                foreach (var table in Histograms)
                    foreach (var attribute in table.Value)
                        returnList.Add($"{table.Key}.{attribute.Key}");

                return returnList;
            }
        }

        protected IDataGatherer DataGatherer { get; set; }
        public BaseHistogramManager(IDataGatherer dataGatherer)
        {
            DataGatherer = dataGatherer;
        }

        public async Task<List<Task>> AddHistogramsFromDB()
        {
            ClearHistograms();
            List<Task> tasks = new List<Task>();
            foreach (string tableName in await DataGatherer.GetTableNamesInSchema())
            {
                foreach (string attributeName in (await DataGatherer.GetAttributeNamesForTable(tableName)))
                    tasks.Add(AddHistogramForAttribute(attributeName, tableName));
            }

            return tasks;
        }

        public void AddHistogram(IHistogram histogram)
        {
            if (string.IsNullOrWhiteSpace(histogram.TableName))
                throw new ArgumentException("Table name cannot be empty!");
            if (string.IsNullOrWhiteSpace(histogram.AttributeName))
                throw new ArgumentException("Attribute name cannot be empty!");

            string tableName = histogram.TableName.ToLower();
            string attributeName = histogram.AttributeName.ToLower();

            if (!Histograms.ContainsKey(tableName))
                Histograms.Add(tableName, new Dictionary<string, IHistogram>());

            Histograms[tableName]
                .Add(attributeName, histogram);
        }

        public void ClearHistograms()
        {
            Histograms.Clear();
        }

        public IHistogram GetHistogram(string tableName, string attributeName)
        {
            tableName = tableName.ToLower();
            attributeName = attributeName.ToLower();

            Dictionary<string, IHistogram> table;
            if (!Histograms.TryGetValue(tableName, out table!))
                throw new KeyNotFoundException($"No histograms found for table '{tableName}'");

            IHistogram histogram;
            if (!table.TryGetValue(attributeName, out histogram!))
                throw new KeyNotFoundException($"Attribute '{attributeName}' not found for table '{tableName}'");

            return histogram;
        }

        public List<IHistogram> GetHistogramsByTable(string table)
        {
            table = table.ToLower();
            if (!Histograms.ContainsKey(table))
                return new List<IHistogram>();

            return Histograms[table].Values.ToList();
        }
        public List<IHistogram> GetHistogramsByAttribute(string attribute)
        {
            return Histograms
                .Select(tableDict => tableDict.Value)
                .Select(table => (IEnumerable<IHistogram>)table.Values.ToList())
                .Aggregate((histograms, histogram) => histogram.Union(histograms))
                .Where(histogram => histogram.AttributeName == attribute)
                .ToList();
        }

        protected abstract Task<IHistogram?> GetCachedHistogramOrNull(string tableName, string attributeName);
        protected abstract Task<IHistogram> CreateHistogramForAttribute(string tableName, string attributeName);

        protected abstract Task CacheHistogram(string tableName, string attributeName, IHistogram histogram);

        protected virtual async Task AddHistogramForAttribute(string attributeName, string tableName)
        {
            var cached = await GetCachedHistogramOrNull(tableName, attributeName);

            if (cached != null)
            {
                AddHistogram(cached);
            }
            else
            {
                var newHistogram = await CreateHistogramForAttribute(tableName, attributeName);
                await CacheHistogram(tableName, attributeName, newHistogram);
                AddHistogram(newHistogram);
            }
        }

        public override string? ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Recorded Histograms:");
            foreach (var histogram in Histograms)
                sb.AppendLine(histogram.ToString());
            return sb.ToString();
        }
    }
}
