﻿using Histograms.DataGatherers;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Stubs
{
    internal class DataGathererStub : IDataGatherer
    {
        public Task<IEnumerable<string>> GetAttributeNamesForTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ValueCount>> GetSortedGroupsFromDb(string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetTableNamesInSchema()
        {
            throw new NotImplementedException();
        }
    }
}