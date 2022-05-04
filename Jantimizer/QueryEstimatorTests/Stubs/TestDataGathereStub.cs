using Histograms.DataGatherers;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Stubs
{
    internal class TestDataGathereStub : IDataGatherer
    {
        public Dictionary<TableAttribute, AttributeData> Data { get; set; } = new Dictionary<TableAttribute, AttributeData>();

        public Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public Task<Type> GetAttributeType(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        public Task<Type> GetAttributeType(TableAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public Task<AttributeData> GetData(TableAttribute attribute)
        {
            return Task.Run(() => Data[attribute]);
        }

        public Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ValueCount>> GetSortedGroupsFromDb(TableAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTableAttributeColumnHash(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetTableNamesInSchema()
        {
            throw new NotImplementedException();
        }
    }
}
