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

        #region GetBucketCertainty

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(1, 2, 0.5)]
        [DataRow(1, 10, 0.1)]
        [DataRow(100, 10, 10)]
        public void Can_GetBucketCertainty(double stdDeviation, double range, double expResult)
        {
            // ARRANGE
            HistogramBucketVariance bucket = new HistogramBucketVariance(0, 1, 1, 1, 1, stdDeviation, range);

            var joinCost = new JoinEstimateEquiDepthVariance();

            // ACT
            double bCertainty = joinCost.GetBucketCertainty(bucket);

            // ASSERTs
            Assert.AreEqual(expResult, bCertainty);
        }

        [TestMethod]
        public void Can_GetBucketCertainty_If_Bucket_RangeZero()
        {
            // ARRANGE
            HistogramBucketVariance bucket = new HistogramBucketVariance(0, 1, 1, 1, 1, 1, 0);

            var joinCost = new JoinEstimateEquiDepthVariance();

            // ACT
            double bCertainty = joinCost.GetBucketCertainty(bucket);

            // ASSERTs
            Assert.AreEqual(1, bCertainty);
        }

        [TestMethod]
        public void Can_GetBucketCertainty_If_Bucket_StandardDeviationZero()
        {
            // ARRANGE
            HistogramBucketVariance bucket = new HistogramBucketVariance(0, 1, 1, 1, 1, 0, 1);

            var joinCost = new JoinEstimateEquiDepthVariance();

            // ACT
            double bCertainty = joinCost.GetBucketCertainty(bucket);

            // ASSERTs
            Assert.AreEqual(1, bCertainty);
        }

        #endregion

        #region GetTotalCertainty

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(1, 2, 0.5)]
        [DataRow(2, 1, 0.5)]
        [DataRow(100, 1, 0.01)]
        public void Can_GetTotalCertainty(double bucketCertainty, double comparisonBucketCertainty, double expResult)
        {
            // ARRANGE
            var joinCost = new JoinEstimateEquiDepthVariance();

            // ACT
            double certainty = joinCost.GetTotalCertainty(bucketCertainty, comparisonBucketCertainty);

            // ASSERTs
            Assert.AreEqual(expResult, certainty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Cant_GetTotalCertainty_IfComparisonIsZero()
        {
            // ARRANGE
            var joinCost = new JoinEstimateEquiDepthVariance();

            // ACT
            joinCost.GetTotalCertainty(1, 0);
        }

        #endregion
    }
}