using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Cost.EstimateCalculators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest.Cost.EstimateCalculators
{
    [TestClass]
    public class BoundsFinderTests
    {

        //#region GetMatchBucketIndex

        //[TestMethod]
        //[DataRow(
        //    new int[] { 1, 11, 21 },
        //    new int[] { 10, 20, 30 },
        //    0,
        //    3,
        //    15,
        //    1)]
        //[DataRow(
        //    new int[] { 1, 11, 21 },
        //    new int[] { 10, 20, 30 },
        //    0,
        //    3,
        //    5,
        //    0)]
        //[DataRow(
        //    new int[] { 1, 11, 21 },
        //    new int[] { 10, 20, 30 },
        //    0,
        //    3,
        //    25,
        //    2)]
        //public void Can_GetMatchBucketIndex_Found(int[] startIndexes, int[] endIndexes, int lowerBound, int upperBound, IComparable compareValue, int expectedIndex)
        //{
        //    // ARRANGE
        //    var indexList = new List<IHistogramBucket>();
        //    for (int i = 0; i < startIndexes.Length; i++)
        //        indexList.Add(new HistogramBucket(startIndexes[i], endIndexes[i], 1));

        //    // ACT
        //    var result = BoundsFinder.GetMatchBucketIndex(indexList, lowerBound, upperBound, compareValue);

        //    // ASSERT
        //    Assert.AreEqual(expectedIndex, result);
        //}

        //[TestMethod]
        //[DataRow(
        //    new int[] { 1, 11, 21 },
        //    new int[] { 10, 20, 30 },
        //    0,
        //    3,
        //    50,
        //    -1)]
        //[DataRow(
        //    new int[] { 1, 11, 21 },
        //    new int[] { 10, 20, 30 },
        //    0,
        //    3,
        //    0,
        //    -1)]
        //public void Can_GetMatchBucketIndex_NotFound(int[] startIndexes, int[] endIndexes, int lowerBound, int upperBound, IComparable compareValue, int expectedIndex)
        //{
        //    // ARRANGE
        //    var indexList = new List<IHistogramBucket>();
        //    for (int i = 0; i < startIndexes.Length; i++)
        //        indexList.Add(new HistogramBucket(startIndexes[i], endIndexes[i], 1));

        //    // ACT
        //    var result = BoundsFinder.GetMatchBucketIndex(indexList, lowerBound, upperBound, compareValue);

        //    // ASSERT
        //    Assert.AreEqual(expectedIndex, result);
        //}

        //#endregion

        //#region GetBucketBounds

        //[TestMethod]
        //[DataRow(
        //    new int[] { 1, 11 },
        //    new int[] { 10, 20 },
        //    new int[] { 5, 31 },
        //    new int[] { 30, 40 },
        //    2,
        //    2)]
        //[DataRow(
        //    new int[] { 1, 100 },
        //    new int[] { 10, 110 },
        //    new int[] { 5, 31 },
        //    new int[] { 30, 40 },
        //    0,
        //    0)]
        //public void Can_GetBucketBounds_Less(int[] leftBucketsStart, int[] leftBucketsEnd, int[] rightBucketsStart, int[] rightBucketsEnd, int expLeftCount, int expRightCount)
        //{
        //    // ARRANGE
        //    var bucketsLeft = new List<IHistogramBucket>();
        //    var rightLeft = new List<IHistogramBucket>();
        //    for (int i = 0; i < leftBucketsStart.Length; i++)
        //        bucketsLeft.Add(new HistogramBucket(leftBucketsStart[i], leftBucketsEnd[i], 1));
        //    for (int i = 0; i < rightBucketsStart.Length; i++)
        //        rightLeft.Add(new HistogramBucket(rightBucketsStart[i], rightBucketsEnd[i], 1));

        //    // ACT
        //    var result = BoundsFinder.GetBucketBounds(ComparisonType.Type.Less, bucketsLeft, rightLeft);

        //    // ASSERT
        //    Assert.AreEqual(expLeftCount, result.LeftBuckets.Count);
        //    Assert.AreEqual(expRightCount, result.RightBuckets.Count);
        //}

        //[TestMethod]
        //[DataRow(
        //    new int[] { 1, 11, 32 },
        //    new int[] { 10, 31, 200 },
        //    new int[] { 5, 31 },
        //    new int[] { 30, 40 },
        //    3,
        //    2)]
        //[DataRow(
        //    new int[] { 1, 11 },
        //    new int[] { 10, 31 },
        //    new int[] { 1, 31, 41 },
        //    new int[] { 10, 40, 500 },
        //    2,
        //    3)]
        //public void Can_GetBucketBounds_LessOrEqual(int[] leftBucketsStart, int[] leftBucketsEnd, int[] rightBucketsStart, int[] rightBucketsEnd, int expLeftCount, int expRightCount)
        //{
        //    // ARRANGE
        //    var bucketsLeft = new List<IHistogramBucket>();
        //    var rightLeft = new List<IHistogramBucket>();
        //    for (int i = 0; i < leftBucketsStart.Length; i++)
        //        bucketsLeft.Add(new HistogramBucket(leftBucketsStart[i], leftBucketsEnd[i], 1));
        //    for (int i = 0; i < rightBucketsStart.Length; i++)
        //        rightLeft.Add(new HistogramBucket(rightBucketsStart[i], rightBucketsEnd[i], 1));

        //    // ACT
        //    var result = BoundsFinder.GetBucketBounds(ComparisonType.Type.EqualOrLess, bucketsLeft, rightLeft);

        //    // ASSERT
        //    Assert.AreEqual(expLeftCount, result.LeftBuckets.Count);
        //    Assert.AreEqual(expRightCount, result.RightBuckets.Count);
        //}

        //[TestMethod]
        //[DataRow(
        //    new int[] { 1, 11 },
        //    new int[] { 10, 20 },
        //    new int[] { 21, 31 },
        //    new int[] { 30, 40 },
        //    0,
        //    0)]
        //[DataRow(
        //    new int[] { 1, 100 },
        //    new int[] { 10, 110 },
        //    new int[] { 5, 31 },
        //    new int[] { 30, 40 },
        //    2,
        //    2)]
        //public void Can_GetBucketBounds_More(int[] leftBucketsStart, int[] leftBucketsEnd, int[] rightBucketsStart, int[] rightBucketsEnd, int expLeftCount, int expRightCount)
        //{
        //    // ARRANGE
        //    var bucketsLeft = new List<IHistogramBucket>();
        //    var rightLeft = new List<IHistogramBucket>();
        //    for (int i = 0; i < leftBucketsStart.Length; i++)
        //        bucketsLeft.Add(new HistogramBucket(leftBucketsStart[i], leftBucketsEnd[i], 1));
        //    for (int i = 0; i < rightBucketsStart.Length; i++)
        //        rightLeft.Add(new HistogramBucket(rightBucketsStart[i], rightBucketsEnd[i], 1));

        //    // ACT
        //    var result = BoundsFinder.GetBucketBounds(ComparisonType.Type.More, bucketsLeft, rightLeft);

        //    // ASSERT
        //    Assert.AreEqual(expLeftCount, result.LeftBuckets.Count);
        //    Assert.AreEqual(expRightCount, result.RightBuckets.Count);
        //}

        //[TestMethod]
        //[DataRow(
        //    new int[] { 1, 11, 32 },
        //    new int[] { 10, 31, 200 },
        //    new int[] { 5, 31 },
        //    new int[] { 30, 40 },
        //    3,
        //    2)]
        //[DataRow(
        //    new int[] { 1, 11 },
        //    new int[] { 10, 31 },
        //    new int[] { 1, 31, 41 },
        //    new int[] { 10, 40, 500 },
        //    2,
        //    2)]
        //public void Can_GetBucketBounds_MoreOrEqual(int[] leftBucketsStart, int[] leftBucketsEnd, int[] rightBucketsStart, int[] rightBucketsEnd, int expLeftCount, int expRightCount)
        //{
        //    // ARRANGE
        //    var bucketsLeft = new List<IHistogramBucket>();
        //    var rightLeft = new List<IHistogramBucket>();
        //    for (int i = 0; i < leftBucketsStart.Length; i++)
        //        bucketsLeft.Add(new HistogramBucket(leftBucketsStart[i], leftBucketsEnd[i], 1));
        //    for (int i = 0; i < rightBucketsStart.Length; i++)
        //        rightLeft.Add(new HistogramBucket(rightBucketsStart[i], rightBucketsEnd[i], 1));

        //    // ACT
        //    var result = BoundsFinder.GetBucketBounds(ComparisonType.Type.EqualOrMore, bucketsLeft, rightLeft);

        //    // ASSERT
        //    Assert.AreEqual(expLeftCount, result.LeftBuckets.Count);
        //    Assert.AreEqual(expRightCount, result.RightBuckets.Count);
        //}

        //#endregion
    }
}
