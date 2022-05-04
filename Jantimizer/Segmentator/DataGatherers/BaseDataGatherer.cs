using Milestoner.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Milestoner.DataGatherers
{
    public abstract class BaseDataGatherer : IDataGatherer
    {
        public abstract Task<IEnumerable<string>> GetTableNamesInSchema();
        public abstract Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName);
        public abstract Task<List<ValueCount>> GetSortedGroupsFromDb(TableAttribute attribute);

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
