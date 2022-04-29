using Histograms.DataGatherers;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace HistogramsTests.Stubs
{
    internal class DataGathererStub : IDataGatherer
    {
        public Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public Task<Type> GetAttributeType(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        public Task<AttributeData> GetData(TableAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public Task<IHistogram?> GetHistogramFromCacheOrNull(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName)
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

        Task<IEnumerable<string>> IDataGatherer.GetAttributeNamesForTable(string tableName)
        {
            throw new NotImplementedException();
        }

        Task<Type> IDataGatherer.GetAttributeType(TableAttribute attribute)
        {
            throw new NotImplementedException();
        }

        Task<Type> IDataGatherer.GetAttributeType(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        Task<List<ValueCount>> IDataGatherer.GetSortedGroupsFromDb(TableAttribute attribute)
        {
            throw new NotImplementedException();
        }

        Task<List<ValueCount>> IDataGatherer.GetSortedGroupsFromDb(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        Task<string> IDataGatherer.GetTableAttributeColumnHash(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<string>> IDataGatherer.GetTableNamesInSchema()
        {
            throw new NotImplementedException();
        }
    }
}
