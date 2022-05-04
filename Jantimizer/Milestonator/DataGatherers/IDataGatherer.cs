using Milestoner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Milestoner.DataGatherers
{
    public interface IDataGatherer
    {
        public Task<IEnumerable<string>> GetTableNamesInSchema();
        public Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName);
        public Task<List<ValueCount>> GetSortedGroupsFromDb(TableAttribute attribute);
    }
}
