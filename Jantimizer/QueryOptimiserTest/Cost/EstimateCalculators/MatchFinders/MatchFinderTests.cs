using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Cost.Nodes;
using QueryOptimiser.Models;
using QueryOptimiserTest.Stubs;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryOptimiser.Cost.EstimateCalculators.MatchFinders;

namespace QueryOptimiserTest.Cost.EstimateCalculators.MatchFinders
{
    [TestClass]
    public class MatchFinderTests
    {
        #region DoesOverlap

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
        public void Can_DoesOverlap_TrueCases(int leftStart, int leftEnd, int rightStart, int rightEnd)
        {
            // ARRANGE
            IHistogramBucket leftBucket = new HistogramBucket(leftStart, leftEnd, 1);
            IHistogramBucket rightBucket = new HistogramBucket(rightStart, rightEnd, 1);
            var estimator = new JoinMatchFinder(new JoinEstimateEquiDepth());

            // ACT
            var result = estimator.DoesOverlap(leftBucket, rightBucket);

            // ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(0, 10, 20, 30)]
        [DataRow(20, 30, 0, 10)]
        public void Can_DoesOverlap_FalseCases(int leftStart, int leftEnd, int rightStart, int rightEnd)
        {
            // ARRANGE
            IHistogramBucket leftBucket = new HistogramBucket(leftStart, leftEnd, 1);
            IHistogramBucket rightBucket = new HistogramBucket(rightStart, rightEnd, 1);
            var estimator = new JoinMatchFinder(new JoinEstimateEquiDepth());

            // ACT
            var result = estimator.DoesOverlap(leftBucket, rightBucket);

            // ASSERT
            Assert.IsFalse(result);
        }

        #endregion

        #region MakeNewIntermediateBucket

        [TestMethod]
        public void Can_MakeNewIntermediateBucket()
        {
            // ARRANGE
            JoinPredicate predicate = new JoinPredicate(
                new TableReferenceNode(0, "a", "a"),
                "v",
                new TableReferenceNode(1, "b", "b"),
                "v",
                "a.v < b.v",
                ComparisonType.Type.Less);
            IHistogramBucket leftBucket = new HistogramBucket(0, 10, 5);
            IHistogramBucket rightBucket = new HistogramBucket(10, 20, 5);

            var estimator = new JoinMatchFinder(new JoinEstimateEquiDepth());

            // ACT
            var result = estimator.MakeNewIntermediateBucket(JoinMatchFinder.MatchType.Match, predicate, leftBucket, rightBucket);

            // ASSERT
            Assert.AreEqual(leftBucket, result.Buckets[new TableAttribute("a", "v")].Bucket);
            Assert.AreEqual(rightBucket, result.Buckets[new TableAttribute("b", "v")].Bucket);
            Assert.IsTrue(result.Buckets.Keys.Contains(new TableAttribute("a", "v")));
            Assert.IsTrue(result.Buckets.Keys.Contains(new TableAttribute("b", "v")));
        }

        #endregion

        #region GetInEqualityMatches

        [TestMethod]
        [DataRow(
            // Left buckets
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            // Right buckets
            new int[] { 1, 6, 31 },
            new int[] { 5, 30, 50 },
            4)]
        [DataRow(
            // Left buckets
            new int[] { 1, 3, 5 },
            new int[] { 2, 4, 6 },
            // Right buckets
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            3)]
        public void Can_GetInEqualityMatches_More(int[] leftFrom, int[] leftTo, int[] rightFrom, int[] rightTo, int expIntermediateBucketCount)
        {
            // ARRANGE
            JoinPredicate predicate = new JoinPredicate(
                new TableReferenceNode(0, "a", "a"),
                "v",
                new TableReferenceNode(1, "b", "b"),
                "v",
                "a.v > b.v",
                ComparisonType.Type.More);
            var bucketsLeft = new List<IHistogramBucket>();
            var bucketsRight = new List<IHistogramBucket>();
            for (int i = 0; i < leftFrom.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftFrom[i], leftTo[i], 1));
            for (int i = 0; i < rightFrom.Length; i++)
                bucketsRight.Add(new HistogramBucket(rightFrom[i], rightTo[i], 1));

            var estimator = new JoinMatchFinder(new JoinEstimateEquiDepth());

            // ACT
            var result = estimator.GetInEqualityMatches(predicate, bucketsLeft, bucketsRight);

            // ASSERT
            Assert.AreEqual(expIntermediateBucketCount, result.Count);
        }

        [TestMethod]
        [DataRow(
            // Left buckets
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            // Right buckets
            new int[] { 2, 11, 31 },
            new int[] { 10, 30, 50 },
            4)]
        [DataRow(
            // Left buckets
            new int[] { 1, 3, 5 },
            new int[] { 2, 20, 6 },
            // Right buckets
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            3)]
        public void Can_GetInEqualityMatches_MoreOrEqual(int[] leftFrom, int[] leftTo, int[] rightFrom, int[] rightTo, int expIntermediateBucketCount)
        {
            // ARRANGE
            JoinPredicate predicate = new JoinPredicate(
                new TableReferenceNode(0, "a", "a"),
                "v",
                new TableReferenceNode(1, "b", "b"),
                "v",
                "a.v >= b.v",
                ComparisonType.Type.EqualOrMore);
            var bucketsLeft = new List<IHistogramBucket>();
            var bucketsRight = new List<IHistogramBucket>();
            for (int i = 0; i < leftFrom.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftFrom[i], leftTo[i], 1));
            for (int i = 0; i < rightFrom.Length; i++)
                bucketsRight.Add(new HistogramBucket(rightFrom[i], rightTo[i], 1));

            var estimator = new JoinMatchFinder(new JoinEstimateEquiDepth());

            // ACT
            var result = estimator.GetInEqualityMatches(predicate, bucketsLeft, bucketsRight);

            // ASSERT
            Assert.AreEqual(expIntermediateBucketCount, result.Count);
        }

        [TestMethod]
        [DataRow(
            // Left buckets
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            // Right buckets
            new int[] { 2, 11, 31 },
            new int[] { 10, 30, 50 },
            3)]
        [DataRow(
            // Left buckets
            new int[] { 1, 3, 5 },
            new int[] { 2, 20, 6 },
            // Right buckets
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            9)]
        public void Can_GetInEqualityMatches_Less(int[] leftFrom, int[] leftTo, int[] rightFrom, int[] rightTo, int expIntermediateBucketCount)
        {
            // ARRANGE
            JoinPredicate predicate = new JoinPredicate(
                new TableReferenceNode(0, "a", "a"),
                "v",
                new TableReferenceNode(1, "b", "b"),
                "v",
                "a.v < b.v",
                ComparisonType.Type.Less);
            var bucketsLeft = new List<IHistogramBucket>();
            var bucketsRight = new List<IHistogramBucket>();
            for (int i = 0; i < leftFrom.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftFrom[i], leftTo[i], 1));
            for (int i = 0; i < rightFrom.Length; i++)
                bucketsRight.Add(new HistogramBucket(rightFrom[i], rightTo[i], 1));

            var estimator = new JoinMatchFinder(new JoinEstimateEquiDepth());

            // ACT
            var result = estimator.GetInEqualityMatches(predicate, bucketsLeft, bucketsRight);

            // ASSERT
            Assert.AreEqual(expIntermediateBucketCount, result.Count);
        }

        [TestMethod]
        [DataRow(
            // Left buckets
            new int[] { 9, 11, 21 },
            new int[] { 10, 20, 30 },
            // Right buckets
            new int[] { 2, 10, 31 },
            new int[] { 9, 30, 32 },
            3)]
        [DataRow(
            // Left buckets
            new int[] { 1, 11, 5 },
            new int[] { 10, 20, 6 },
            // Right buckets
            new int[] { 1, 11, 21 },
            new int[] { 5, 20, 30 },
            6)]
        public void Can_GetInEqualityMatches_LessOrEqual(int[] leftFrom, int[] leftTo, int[] rightFrom, int[] rightTo, int expIntermediateBucketCount)
        {
            // ARRANGE
            JoinPredicate predicate = new JoinPredicate(
                new TableReferenceNode(0, "a", "a"),
                "v",
                new TableReferenceNode(1, "b", "b"),
                "v",
                "a.v <= b.v",
                ComparisonType.Type.EqualOrLess);
            var bucketsLeft = new List<IHistogramBucket>();
            var bucketsRight = new List<IHistogramBucket>();
            for (int i = 0; i < leftFrom.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftFrom[i], leftTo[i], 1));
            for (int i = 0; i < rightFrom.Length; i++)
                bucketsRight.Add(new HistogramBucket(rightFrom[i], rightTo[i], 1));

            var estimator = new JoinMatchFinder(new JoinEstimateEquiDepth());

            // ACT
            var result = estimator.GetInEqualityMatches(predicate, bucketsLeft, bucketsRight);

            // ASSERT
            Assert.AreEqual(expIntermediateBucketCount, result.Count);
        }

        #endregion

        #region GetEqualityMatches

        [TestMethod]
        [DataRow(
            // Left buckets
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            // Right buckets
            new int[] { 1, 6, 31 },
            new int[] { 5, 30, 50 },
            4)]
        [DataRow(
            // Left buckets
            new int[] { 1, 3, 5 },
            new int[] { 2, 4, 6 },
            // Right buckets
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            3)]
        public void Can_GetEqualityMatches(int[] leftFrom, int[] leftTo, int[] rightFrom, int[] rightTo, int expIntermediateBucketCount)
        {
            // ARRANGE
            JoinPredicate predicate = new JoinPredicate(
                new TableReferenceNode(0, "a", "a"),
                "v",
                new TableReferenceNode(1, "b", "b"),
                "v",
                "a.v > b.v",
                ComparisonType.Type.More);
            var bucketsLeft = new List<IHistogramBucket>();
            var bucketsRight = new List<IHistogramBucket>();
            for (int i = 0; i < leftFrom.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftFrom[i], leftTo[i], 1));
            for (int i = 0; i < rightFrom.Length; i++)
                bucketsRight.Add(new HistogramBucket(rightFrom[i], rightTo[i], 1));

            var estimator = new JoinMatchFinder(new JoinEstimateEquiDepth());

            // ACT
            var result = estimator.GetEqualityMatches(predicate, bucketsLeft, bucketsRight);

            // ASSERT
            Assert.AreEqual(expIntermediateBucketCount, result.Count);
        }

        #endregion
    }
}
