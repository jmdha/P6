using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.HistogramModels
{
    [TestClass]
    public class HistogramBucketTests
    {
        #region Constructor

        [TestMethod]
        [DataRow(1, 5, 10)]
        [DataRow(111, 112, 1000)]
        [DataRow(1, 50000, 10)]
        [DataRow(1, 5, 110000)]
        public void Constructor_SetsProperties(int start, int stop, int count)
        {
            // ARRANGE
            // ACT
            IHistogramBucket bucket = new HistogramBucket(start, stop, count);

            // ASSERT
            Assert.AreEqual(start, bucket.ValueStart);
            Assert.AreEqual(stop, bucket.ValueEnd);
            Assert.AreEqual(count, bucket.Count);
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
            new HistogramBucket(start, stop, 10);
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
            new HistogramBucket(1, 2, count);
        }

        #endregion
    }
}
