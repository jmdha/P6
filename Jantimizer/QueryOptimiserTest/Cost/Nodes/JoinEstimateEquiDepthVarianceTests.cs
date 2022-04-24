using Histograms;
using Histograms.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiserTest;
using QueryParser.Models;
using System;
using QueryOptimiser.Cost.Nodes;
using Histograms.Models;

namespace QueryOptimiserTest.Cost.Nodes
{
    [TestClass]
    public class JoinEstimateEquiDepthVarianceTests
    {
        #region GetBucketEstimate

        [TestMethod]
        [DataRow(ComparisonType.Type.Equal, 10, 10, 0, 0, 0, 0, 10)]
        [DataRow(ComparisonType.Type.Less, 10, 10, 0, 0, 0, 0, 10)]
        [DataRow(ComparisonType.Type.More, 10, 10, 0, 0, 0, 0, 10)]
        [DataRow(ComparisonType.Type.EqualOrLess, 10, 10, 0, 0, 0, 0, 10)]
        [DataRow(ComparisonType.Type.EqualOrMore, 10, 10, 0, 0, 0, 0, 10)]
        [DataRow(ComparisonType.Type.Equal, 100, 100, 25, 25, 25, 25, 100)]
        [DataRow(ComparisonType.Type.Equal, 100, 100, 10, 50, 10, 0, 100)]
        [DataRow(ComparisonType.Type.Equal, 100, 100, 25, 100, 100, 100, 25)]
        [DataRow(ComparisonType.Type.Equal, 100, 100, 100, 100, 25, 100, 25)]
        [DataRow(ComparisonType.Type.Equal, 100, 50, 25, 10, 100, 50, 80)]

        public void Can_GetBucketEstimate(ComparisonType.Type predicate, int aAmount, int bAmount, double aStd, double bStd, double aRange, double bRange, int expectedEstimate)
        {
            // ARRANGE
            IHistogramBucket leftBucket = new HistogramBucketVariance(0, 0, aAmount, 0, 0, aStd, aRange);
            IHistogramBucket rightBucket = new HistogramBucketVariance(0, 0, bAmount, 0, 0, bStd, bRange);

            var joinCost = new JoinEstimateEquiDepthVariance();

            // ACT
            long estimate = joinCost.GetBucketEstimate(predicate, leftBucket, rightBucket);

            // ASSERTs
            Assert.AreEqual(expectedEstimate, estimate);
        }

        #endregion
    }
}