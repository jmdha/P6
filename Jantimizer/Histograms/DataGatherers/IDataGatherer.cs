using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.DataGatherers
{
    public interface IDataGatherer
    {
        public Task<IEnumerable<string>> GetTableNamesInSchema();
        public Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName);
        public Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName);
        public Task<string> GetTableAttributeColumnHash(string tableName, string attributeName);
    }
}
