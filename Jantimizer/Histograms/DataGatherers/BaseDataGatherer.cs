using Histograms.Caches;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.DataGatherers
{
    public abstract class BaseDataGatherer : IDataGatherer
    {
        public abstract Task<IEnumerable<string>> GetTableNamesInSchema();
        public abstract Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName);
        public abstract Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName);
        public abstract Task<string> GetTableAttributeColumnHash(string tableName, string attributeName);

        public async Task<IHistogram?> GetHistogramFromCacheOrNull(string tableName, string attributeName)
        {
            if (HistogramCacher.Instance == null)
                return null;
            string hash = await GetTableAttributeColumnHash(tableName, attributeName);
            var retVal = HistogramCacher.Instance.GetValueOrNull(new string[] { tableName, attributeName, hash });
            return retVal;
        }

        protected IEnumerable<ValueCount> GetValueCounts(DataSet dataSet, string valueColumnName, string countColumnName)
        {
            return dataSet.Tables[0].AsEnumerable().Select(r =>
                new ValueCount(
                    (IComparable)r[valueColumnName],
                    (long)r[countColumnName]
                )
            );
        }
    }
}
