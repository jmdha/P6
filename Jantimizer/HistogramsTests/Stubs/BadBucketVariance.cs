using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Stubs
{
    internal class BadBucketVariance : IHistogramBucketVariance
    {
        public IComparable ValueStart => throw new NotImplementedException();

        public IComparable ValueEnd => throw new NotImplementedException();

        public long Count => throw new NotImplementedException();

        public decimal Variance => throw new NotImplementedException();

        public decimal Mean => throw new NotImplementedException();

        public decimal StandardDeviation => throw new NotImplementedException();
    }
}
