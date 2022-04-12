using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Stubs
{
    internal class BadHistogram : IHistogram
    {
        public string TableName => throw new NotImplementedException();

        public string AttributeName => throw new NotImplementedException();

        public List<IHistogramBucket> Buckets => throw new NotImplementedException();

        public List<TypeCode> AcceptedTypes => throw new NotImplementedException();

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public void GenerateHistogram(DataTable table, string key)
        {
            throw new NotImplementedException();
        }

        public void GenerateHistogram(List<IComparable> column)
        {
            throw new NotImplementedException();
        }

        public void GenerateHistogramFromSortedGroups(IEnumerable<ValueCount> sortedGroups)
        {
            throw new NotImplementedException();
        }
    }
}
