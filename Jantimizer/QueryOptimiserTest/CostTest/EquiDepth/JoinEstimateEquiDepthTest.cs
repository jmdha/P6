using Histograms;
using Histograms.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiserTest;
using QueryParser.Models;
using System;
using QueryOptimiserTest.Stubs;
using QueryOptimiser.Cost.Nodes;
using Histograms.Models;

namespace CostEquiDepthTest;

[TestClass]
public class JoinEstimateEquiDepthTest
{
    [TestMethod]
    [DataRow(ComparisonType.Type.Equal, 10, 10, 10)]
    [DataRow(ComparisonType.Type.Equal, 100, 10, 100)]
    [DataRow(ComparisonType.Type.Less, 100, 10, 100)]
    [DataRow(ComparisonType.Type.More, 100, 10, 100)]
    [DataRow(ComparisonType.Type.EqualOrLess, 100, 10, 100)]
    [DataRow(ComparisonType.Type.EqualOrMore, 100, 10, 100)]
    public void BucketEstimateTest(ComparisonType.Type predicate, int aAmount, int bAmount, int expectedEstimate)
    {
        // ARRANGE
        IHistogramBucket leftBucket = new HistogramBucket(0, 0, aAmount);
        IHistogramBucket rightBucket = new HistogramBucket(0, 0, bAmount);

        var joinCost = new JoinEstimateEquiDepth();

        // ACT
        long estimate = joinCost.GetBucketEstimate(predicate, leftBucket, rightBucket);

        // ASSERTs
        Assert.AreEqual(expectedEstimate, estimate);
    }
}