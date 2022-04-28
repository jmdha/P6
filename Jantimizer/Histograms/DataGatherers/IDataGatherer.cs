using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Histograms.DataGatherers
{
    public interface IDataGatherer
    {
        public Task<IEnumerable<string>> GetTableNamesInSchema();
        public Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName);
        public Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName);
        public Task<List<ValueCount>> GetSortedGroupsFromDb(TableAttribute attribute);
        public Task<string> GetTableAttributeColumnHash(string tableName, string attributeName);
        public Task<Type> GetAttributeType(string tableName, string attributeName);
        public Task<Type> GetAttributeType(TableAttribute attribute);
        internal Task<AttributeData> GetData(TableAttribute attribute);
    }
}
