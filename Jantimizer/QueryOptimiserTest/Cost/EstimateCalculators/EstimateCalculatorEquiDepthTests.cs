using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiserTest.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest.Cost.EstimateCalculators
{
    [TestClass]
    public class EstimateCalculatorEquiDepthTests
    {
        #region DoesMatch

        [TestMethod]
        // Right bucket start index is within Left bucket range
        // Right Bucket:      |======|
        // Left Bucket:    |======|
        [DataRow(0, 10, 5, 20)]
        [DataRow(0, 1000, 999, 2000)]
        [DataRow(0, 1000, 500, 1001)]
        // Right bucket end index is within Left bucket range
        // Right Bucket:   |======|
        // Left Bucket:       |======|
        [DataRow(50, 100, 0, 51)]
        [DataRow(50, 100, 10, 99)]
        [DataRow(50, 100, 25, 50)]
        // Right bucket is entirely within Left bucket
        // Right Bucket:   |======|
        // Left Bucket:  |===========|
        [DataRow(0, 100, 20, 80)]
        [DataRow(0, 100, 1, 99)]
        // Left bucket is entirely within Right bucket
        // Right Bucket: |===========|
        // Left Bucket:     |=====|
        [DataRow(20, 80, 0, 100)]
        [DataRow(1, 99, 0, 100)]
        public void Can_DoesMatch_TrueCases(int leftStart, int leftEnd, int rightStart, int rightEnd)
        {
            // ARRANGE
            IHistogramBucket leftBucket = new HistogramBucket(leftStart, leftEnd, 1);
            IHistogramBucket rightBucket = new HistogramBucket(rightStart, rightEnd, 1);
            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.DoesMatch(leftBucket, rightBucket);

            // ASSERT
            Assert.IsTrue(result);

        }

        #endregion
    }
}
