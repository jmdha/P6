using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.Models.Buckets
{
    [TestClass]
    public class HistogramBucketVarianceTests
    {
        #region Constructor

        [TestMethod]
        [DataRow(1, 5, 10, 5, 10, 5.2)]
        [DataRow(111, 112, 1000, 10, 5, 34745.21564)]
        [DataRow(1, 50000, 10, 1, 1, 0.000054)]
        [DataRow(1, 5, 110000, 1, 1000, 1)]
        public void Constructor_SetsProperties(int start, int stop, int count, double variance, double mean, double sd)
        {
            // ARRANGE
            // ACT
            IHistogramBucketVariance bucket = new HistogramBucketVariance(start, stop, count, variance, mean, sd);

            // ASSERT
            Assert.AreEqual(start, bucket.ValueStart);
            Assert.AreEqual(stop, bucket.ValueEnd);
            Assert.AreEqual(count, bucket.Count);
            Assert.AreEqual(variance, bucket.Variance);
            Assert.AreEqual(mean, bucket.Mean);
            Assert.AreEqual(sd, bucket.StandardDeviation);
        }

        #endregion

        #region Properties

        [TestMethod]
        [DataRow(5, 1)]
        [DataRow(1000, 999)]
        [DataRow(500000, 1)]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Cant_SetEndValueLessThanStart(int start, int stop)
        {
            // ARRANGE
            // ACT
            new HistogramBucketVariance(start, stop, 10, 0, 0, 0);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-100)]
        [DataRow(-583959)]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Cant_SetCountLessThanZero(int count)
        {
            // ARRANGE
            // ACT
            new HistogramBucketVariance(1, 2, count, 0, 0, 0);
        }

        #endregion
    }
}
