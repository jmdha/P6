using Histograms.Caches;
using Histograms.Models;
using HistogramsTests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.Caches
{
    [TestClass]
    public class CachedBucketTests
    {
        [TestMethod]
        [DataRow(0, 1, 10)]
        [DataRow(10, 999991, 1)]
        [DataRow(0, 20, 199998)]
        public void Constructor_CanSetWith_HistogramBucket(int start, int end, long count)
        {
            // ARRANGE
            HistogramBucket histogramBucket = new HistogramBucket(start, end, count);

            // ACT
            CachedBucket bucket = new CachedBucket(histogramBucket);

            // ASSERT
            Assert.AreEqual(start.ToString(), bucket.ValueStart);
            Assert.AreEqual(end.ToString(), bucket.ValueEnd);
            Assert.AreEqual(count, bucket.Count);
            Assert.AreEqual(0, bucket.Variance);
            Assert.AreEqual(0, bucket.Mean);
            Assert.AreEqual(nameof(HistogramBucket), bucket.TypeName);
        }

        [TestMethod]
        [DataRow(0, 1, 10, 1, 1)]
        [DataRow(10, 999991, 1, 500, 204)]
        [DataRow(0, 20, 199998, 888, 1)]
        public void Constructor_CanSetWith_HistogramBucketVariance(int start, int end, long count, int variance, int mean)
        {
            // ARRANGE
            HistogramBucketVariance histogramBucket = new HistogramBucketVariance(start, end, count, variance, mean);

            // ACT
            CachedBucket bucket = new CachedBucket(histogramBucket);

            // ASSERT
            Assert.AreEqual(start.ToString(), bucket.ValueStart);
            Assert.AreEqual(end.ToString(), bucket.ValueEnd);
            Assert.AreEqual(count, bucket.Count);
            Assert.AreEqual(variance, bucket.Variance);
            Assert.AreEqual(mean, bucket.Mean);
            Assert.AreEqual(nameof(HistogramBucketVariance), bucket.TypeName);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Constructor_ThrowsIfUnknownHistogramType_1()
        {
            CachedBucket bucket = new CachedBucket(new BadBucket());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Constructor_ThrowsIfUnknownHistogramType_2()
        {
            CachedBucket bucket = new CachedBucket(new BadBucketVariance());
        }
    }
}
