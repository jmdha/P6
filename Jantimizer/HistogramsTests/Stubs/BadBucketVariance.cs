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

        public int Variance => throw new NotImplementedException();

        public int Mean => throw new NotImplementedException();
    }
}
