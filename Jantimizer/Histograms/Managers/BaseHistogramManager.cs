using DatabaseConnector;
using Histograms.Caches;
using Histograms.DataGatherers;
using Histograms.Models;
using Histograms.Models.Histograms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Models.JsonModels;

namespace Histograms.Managers
{
    public abstract class BaseHistogramManager : IHistogramManager
    {
        private Dictionary<TableAttribute, IHistogram> Histograms { get; set; } = new Dictionary<TableAttribute, IHistogram>();
        public HistogramSet UsedHistograms { get; }
        public List<string> Tables => Histograms.Keys.Select(a => a.Table.TableName).Distinct().ToList();
        public List<string> Attributes => Histograms.Keys.Select(a => $"{a.Table.TableName}.{a.Attribute}").ToList();

        protected IDataGatherer DataGatherer { get; set; }
        public BaseHistogramManager(IDataGatherer dataGatherer)
        {
            UsedHistograms = new HistogramSet();
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

            await Task.WhenAll(tasks);

            var segmentComparer = new SegmentationComparisonCalculator(DataGatherer);
            await segmentComparer.DoHistogramComparisons(Histograms.Values.ToList());

            return tasks;
        }

        public void AddHistogram(IHistogram histogram)
        {
            if (string.IsNullOrWhiteSpace(histogram.TableName))
                throw new ArgumentException("Table name cannot be empty!");
            if (string.IsNullOrWhiteSpace(histogram.AttributeName))
                throw new ArgumentException("Attribute name cannot be empty!");

            Histograms.Add(histogram.TableAttribute, histogram);
        }

        public void ClearHistograms()
        {
            Histograms.Clear();
            UsedHistograms.Histograms.Clear();
        }

        public IHistogram GetHistogram(string tableName, string attributeName)
            => GetHistogram(new TableAttribute(tableName, attributeName));
        public IHistogram GetHistogram(TableAttribute attribute)
        {
            IHistogram histogram;
            if (!Histograms.TryGetValue(attribute, out histogram!))
                throw new KeyNotFoundException($"Histogram not found for TableAttribute '{attribute}'");

            UsedHistograms.AddHistogram(histogram);

            return histogram;
        }

        protected abstract Task<IHistogram> CreateHistogramForAttribute(string tableName, string attributeName);

        #region Caching
        protected abstract string[] GetCacheHashString(string tableName, string attributeName, string columnHash);
        protected async Task<IHistogram?> GetCachedHistogramOrNull(string tableName, string attributeName)
        {
            if (HistogramCacher.Instance == null)
                return null;

            tableName = tableName.ToLower();
            attributeName = attributeName.ToLower();

            string columnHash = await DataGatherer.GetTableAttributeColumnHash(tableName, attributeName);
            var cacheHisto = HistogramCacher.Instance.GetValueOrNull(GetCacheHashString(tableName, attributeName, columnHash));

            return cacheHisto;
        }

        protected async Task CacheHistogram(string tableName, string attributeName, IHistogram histogram)
        {
            tableName = tableName.ToLower();
            attributeName = attributeName.ToLower();

            string columnHash = await DataGatherer.GetTableAttributeColumnHash(tableName, attributeName);
            if (HistogramCacher.Instance != null)
                HistogramCacher.Instance.AddToCacheIfNotThere(GetCacheHashString(tableName, attributeName, columnHash), histogram);
        }
        #endregion

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
            foreach (var histogram in Histograms.Values)
                    sb.AppendLine(histogram.ToString());
            return sb.ToString();
        }

        public decimal GetAbstractStorageBytes()
        {
            decimal result = 0;
            // Get all bytes from all segments.
            foreach (var histogram in Histograms.Values)
                foreach (var segments in histogram.Segmentations)
                    result += segments.GetTotalAbstractStorageUse();
            // Converting from bit to bytes
            result = result / 8;
            return result;
        }
    }
}
