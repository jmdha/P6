using Milestoner.DataGatherers;
using Milestoner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace MilestonatorTests.Stubs
{
    internal class TestDataGathere : IDataGatherer
    {
        public Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ValueCount>> GetSortedGroupsFromDb(TableAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetTableNamesInSchema()
        {
            throw new NotImplementedException();
        }

        public Task<TypeCode> GetTypeCodeFromDb(TableAttribute attribute)
        {
            throw new NotImplementedException();
        }
    }
}
