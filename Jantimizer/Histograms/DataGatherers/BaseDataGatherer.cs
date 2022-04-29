using Histograms.Caches;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Histograms.DataGatherers
{
    public abstract class BaseDataGatherer : IDataGatherer
    {
        public abstract Task<IEnumerable<string>> GetTableNamesInSchema();
        public abstract Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName);
        public abstract Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName);
        public Task<List<ValueCount>> GetSortedGroupsFromDb(TableAttribute attribute)
            => GetSortedGroupsFromDb(attribute.Table.TableName, attribute.Attribute);
        public abstract Task<string> GetTableAttributeColumnHash(string tableName, string attributeName);
        public abstract Task<Type> GetAttributeType(string tableName, string attributeName);
        public Task<Type> GetAttributeType(TableAttribute attribute)
            => GetAttributeType(attribute.Table.TableName, attribute.Attribute);

        public async Task<AttributeData> GetData(TableAttribute attribute)
        {
            return new AttributeData(
                attribute,
                await GetSortedGroupsFromDb(attribute),
                Type.GetTypeCode(await GetAttributeType(attribute))
            );
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
