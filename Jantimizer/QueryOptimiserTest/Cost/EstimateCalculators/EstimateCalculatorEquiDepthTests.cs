using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Cost.EstimateCalculators;
using QueryOptimiser.Models;
using QueryOptimiserTest.Stubs;
using QueryParser.Models;
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

        [TestMethod]
        [DataRow(0, 10, 20, 30)]
        [DataRow(20, 30, 0, 10)]
        public void Can_DoesMatch_FalseCases(int leftStart, int leftEnd, int rightStart, int rightEnd)
        {
            // ARRANGE
            IHistogramBucket leftBucket = new HistogramBucket(leftStart, leftEnd, 1);
            IHistogramBucket rightBucket = new HistogramBucket(rightStart, rightEnd, 1);
            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.DoesMatch(leftBucket, rightBucket);

            // ASSERT
            Assert.IsFalse(result);
        }

        #endregion

        #region GetOverlappingReferences

        [TestMethod]
        [DataRow(
            // Left side
            new string[] { "a", "b", "d" },
            new string[] { "v", "v", "v" },
            // Right side
            new string[] { "a", "c" },
            new string[] { "v", "v" },
            // Exp result
            new string[] { "a" },
            new string[] { "v" })]
        [DataRow(
            // Left side
            new string[] { "a", "b", "c" },
            new string[] { "v", "v", "v" },
            // Right side
            new string[] { "a", "c" },
            new string[] { "v", "v" },
            // Exp result
            new string[] { "a", "c" },
            new string[] { "v", "v" })]
        public void Can_GetOverlappingReferences(string[] leftTables, string[] leftAttributes, string[] rightTables, string[] rightAttributes, string[] expTables, string[] expAttributes)
        {
            // ARRANGE
            List<TableAttribute> leftTableAttributes = new List<TableAttribute>();
            List<TableAttribute> rightTableAttributes = new List<TableAttribute>();
            for (int i = 0; i < leftTables.Length; i++)
                leftTableAttributes.Add(new TableAttribute(leftTables[i], leftAttributes[i]));
            for (int i = 0; i < rightTables.Length; i++)
                rightTableAttributes.Add(new TableAttribute(rightTables[i], rightAttributes[i]));
            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.GetOverlappingReferences(leftTableAttributes, rightTableAttributes);

            // ASSERT
            for (int i = 0; i < expTables.Length; i++)
            {
                Assert.AreEqual(expTables[i], result[i].Table);
                Assert.AreEqual(expAttributes[i], result[i].Attribute);
            }
        }

        #endregion

        #region GetHistogram
        
        [TestMethod]
        [DataRow("a", "b")]
        [DataRow("ABA", "v")]
        [DataRow("_", "b")]
        public void Can_GetHistogram(string table, string attribute)
        {
            // ARRANGE
            IHistogram testHistogram = new HistogramEquiDepth(table, attribute, 10);
            HistogramManagerStub manager = new HistogramManagerStub();
            var estimator = new EstimateCalculatorEquiDepth(manager);
            manager.TestStorage.Add(testHistogram);

            // ACT
            var result = estimator.GetHistogram(new TableAttribute(table, attribute));

            // ASSERT
            Assert.AreEqual(testHistogram, result);
        }

        #endregion

        #region GetBucketPair

        [TestMethod]
        public void Can_GetBucketPair_NoReferences()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");
            var tableAtt3 = new TableAttribute("c", "v");
            var tableAtt4 = new TableAttribute("d", "v");

            IHistogram testHistogram1 = new HistogramEquiDepth(tableAtt1.Table, tableAtt1.Attribute, 10);
            IHistogramBucket testHistogram1Bucket1 = new HistogramBucket(0, 10, 10);
            IHistogramBucket testHistogram1Bucket2 = new HistogramBucket(11, 20, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAtt2.Table, tableAtt2.Attribute, 10);
            IHistogramBucket testHistogram2Bucket1 = new HistogramBucket(100, 110, 10);
            IHistogramBucket testHistogram2Bucket2 = new HistogramBucket(111, 120, 10);
            testHistogram2.Buckets.Add(testHistogram2Bucket1);
            testHistogram2.Buckets.Add(testHistogram2Bucket2);

            HistogramManagerStub manager = new HistogramManagerStub();
            manager.TestStorage.Add(testHistogram1);
            manager.TestStorage.Add(testHistogram2);

            IHistogramBucket tableHistogramBucket1 = new HistogramBucket(0, 1, 10);
            IHistogramBucket tableHistogramBucket2 = new HistogramBucket(4, 5, 10);

            IntermediateBucket tableBucket1 = new IntermediateBucket();
            tableBucket1.AddBucketIfNotThere(tableAtt3, new BucketEstimate(tableHistogramBucket1, 1));
            IntermediateBucket tableBucket2 = new IntermediateBucket();
            tableBucket2.AddBucketIfNotThere(tableAtt4, new BucketEstimate(tableHistogramBucket2, 1));

            var table = new IntermediateTable();
            table.Buckets.Add(tableBucket1);
            table.Buckets.Add(tableBucket2);
            table.References.Add(tableAtt3);
            table.References.Add(tableAtt4);

            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketPair(tableAtt1, tableAtt2, table);

            // ASSERT
            Assert.AreEqual(2, result.LeftBuckets.Count);
            Assert.AreEqual(2, result.RightBuckets.Count);
            Assert.IsTrue(result.LeftBuckets.Contains(testHistogram1Bucket1));
            Assert.IsTrue(result.LeftBuckets.Contains(testHistogram1Bucket2));
            Assert.IsTrue(result.RightBuckets.Contains(testHistogram2Bucket1));
            Assert.IsTrue(result.RightBuckets.Contains(testHistogram2Bucket2));
        }

        [TestMethod]
        public void Can_GetBucketPair_LeftReferences()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");
            var tableAtt3 = new TableAttribute("a", "v");
            var tableAtt4 = new TableAttribute("d", "v");

            IHistogram testHistogram1 = new HistogramEquiDepth(tableAtt1.Table, tableAtt1.Attribute, 10);
            IHistogramBucket testHistogram1Bucket1 = new HistogramBucket(0, 10, 10);
            IHistogramBucket testHistogram1Bucket2 = new HistogramBucket(11, 20, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAtt2.Table, tableAtt2.Attribute, 10);
            IHistogramBucket testHistogram2Bucket1 = new HistogramBucket(100, 110, 10);
            IHistogramBucket testHistogram2Bucket2 = new HistogramBucket(111, 120, 10);
            testHistogram2.Buckets.Add(testHistogram2Bucket1);
            testHistogram2.Buckets.Add(testHistogram2Bucket2);

            HistogramManagerStub manager = new HistogramManagerStub();
            manager.TestStorage.Add(testHistogram1);
            manager.TestStorage.Add(testHistogram2);

            IHistogramBucket tableHistogramBucket1 = new HistogramBucket(0, 1, 10);
            IHistogramBucket tableHistogramBucket2 = new HistogramBucket(4, 5, 10);

            IntermediateBucket tableBucket1 = new IntermediateBucket();
            tableBucket1.AddBucketIfNotThere(tableAtt3, new BucketEstimate(tableHistogramBucket1, 1));
            IntermediateBucket tableBucket2 = new IntermediateBucket();
            tableBucket2.AddBucketIfNotThere(tableAtt4, new BucketEstimate(tableHistogramBucket2, 1));

            var table = new IntermediateTable();
            table.Buckets.Add(tableBucket1);
            table.Buckets.Add(tableBucket2);
            table.References.Add(tableAtt3);
            table.References.Add(tableAtt4);

            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketPair(tableAtt1, tableAtt2, table);

            // ASSERT
            Assert.AreEqual(1, result.LeftBuckets.Count);
            Assert.AreEqual(2, result.RightBuckets.Count);
            Assert.IsTrue(result.LeftBuckets.Contains(tableHistogramBucket1));
            Assert.IsTrue(result.RightBuckets.Contains(testHistogram2Bucket1));
            Assert.IsTrue(result.RightBuckets.Contains(testHistogram2Bucket2));
        }

        [TestMethod]
        public void Can_GetBucketPair_RightReferences()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");
            var tableAtt3 = new TableAttribute("c", "v");
            var tableAtt4 = new TableAttribute("b", "v");

            IHistogram testHistogram1 = new HistogramEquiDepth(tableAtt1.Table, tableAtt1.Attribute, 10);
            IHistogramBucket testHistogram1Bucket1 = new HistogramBucket(0, 10, 10);
            IHistogramBucket testHistogram1Bucket2 = new HistogramBucket(11, 20, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAtt2.Table, tableAtt2.Attribute, 10);
            IHistogramBucket testHistogram2Bucket1 = new HistogramBucket(100, 110, 10);
            IHistogramBucket testHistogram2Bucket2 = new HistogramBucket(111, 120, 10);
            testHistogram2.Buckets.Add(testHistogram2Bucket1);
            testHistogram2.Buckets.Add(testHistogram2Bucket2);

            HistogramManagerStub manager = new HistogramManagerStub();
            manager.TestStorage.Add(testHistogram1);
            manager.TestStorage.Add(testHistogram2);

            IHistogramBucket tableHistogramBucket1 = new HistogramBucket(0, 1, 10);
            IHistogramBucket tableHistogramBucket2 = new HistogramBucket(4, 5, 10);

            IntermediateBucket tableBucket1 = new IntermediateBucket();
            tableBucket1.AddBucketIfNotThere(tableAtt3, new BucketEstimate(tableHistogramBucket1, 1));
            IntermediateBucket tableBucket2 = new IntermediateBucket();
            tableBucket2.AddBucketIfNotThere(tableAtt4, new BucketEstimate(tableHistogramBucket2, 1));

            var table = new IntermediateTable();
            table.Buckets.Add(tableBucket1);
            table.Buckets.Add(tableBucket2);
            table.References.Add(tableAtt3);
            table.References.Add(tableAtt4);

            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketPair(tableAtt1, tableAtt2, table);

            // ASSERT
            Assert.AreEqual(2, result.LeftBuckets.Count);
            Assert.AreEqual(1, result.RightBuckets.Count);
            Assert.IsTrue(result.LeftBuckets.Contains(testHistogram1Bucket1));
            Assert.IsTrue(result.LeftBuckets.Contains(testHistogram1Bucket2));
            Assert.IsTrue(result.RightBuckets.Contains(tableHistogramBucket2));
        }

        [TestMethod]
        public void Can_GetBucketPair_BothReferences()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");
            var tableAtt3 = new TableAttribute("a", "v");
            var tableAtt4 = new TableAttribute("b", "v");

            IHistogram testHistogram1 = new HistogramEquiDepth(tableAtt1.Table, tableAtt1.Attribute, 10);
            IHistogramBucket testHistogram1Bucket1 = new HistogramBucket(0, 10, 10);
            IHistogramBucket testHistogram1Bucket2 = new HistogramBucket(11, 20, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAtt2.Table, tableAtt2.Attribute, 10);
            IHistogramBucket testHistogram2Bucket1 = new HistogramBucket(100, 110, 10);
            IHistogramBucket testHistogram2Bucket2 = new HistogramBucket(111, 120, 10);
            testHistogram2.Buckets.Add(testHistogram2Bucket1);
            testHistogram2.Buckets.Add(testHistogram2Bucket2);

            HistogramManagerStub manager = new HistogramManagerStub();
            manager.TestStorage.Add(testHistogram1);
            manager.TestStorage.Add(testHistogram2);

            IHistogramBucket tableHistogramBucket1 = new HistogramBucket(0, 1, 10);
            IHistogramBucket tableHistogramBucket2 = new HistogramBucket(4, 5, 10);

            IntermediateBucket tableBucket1 = new IntermediateBucket();
            tableBucket1.AddBucketIfNotThere(tableAtt3, new BucketEstimate(tableHistogramBucket1, 1));
            IntermediateBucket tableBucket2 = new IntermediateBucket();
            tableBucket2.AddBucketIfNotThere(tableAtt4, new BucketEstimate(tableHistogramBucket2, 1));

            var table = new IntermediateTable();
            table.Buckets.Add(tableBucket1);
            table.Buckets.Add(tableBucket2);
            table.References.Add(tableAtt3);
            table.References.Add(tableAtt4);

            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketPair(tableAtt1, tableAtt2, table);

            // ASSERT
            Assert.AreEqual(1, result.LeftBuckets.Count);
            Assert.AreEqual(1, result.RightBuckets.Count);
            Assert.IsTrue(result.LeftBuckets.Contains(tableHistogramBucket1));
            Assert.IsTrue(result.RightBuckets.Contains(tableHistogramBucket2));
        }

        #endregion

        #region GetMatchBucketIndex

        [TestMethod]
        [DataRow(
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            0,
            3,
            15,
            1)]
        [DataRow(
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            0,
            3,
            5,
            0)]
        [DataRow(
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            0,
            3,
            25,
            2)]
        public void Can_GetMatchBucketIndex_Found(int[] startIndexes, int[] endIndexes, int lowerBound, int upperBound, IComparable compareValue, int expectedIndex)
        {
            // ARRANGE
            var indexList = new List<IHistogramBucket>();
            for (int i = 0; i < startIndexes.Length; i++)
                indexList.Add(new HistogramBucket(startIndexes[i], endIndexes[i], 1));

            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.GetMatchBucketIndex(indexList, lowerBound, upperBound, compareValue);

            // ASSERT
            Assert.AreEqual(expectedIndex, result);
        }

        [TestMethod]
        [DataRow(
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            0,
            3,
            50,
            -1)]
        [DataRow(
            new int[] { 1, 11, 21 },
            new int[] { 10, 20, 30 },
            0,
            3,
            0,
            -1)]
        public void Can_GetMatchBucketIndex_NotFound(int[] startIndexes, int[] endIndexes, int lowerBound, int upperBound, IComparable compareValue, int expectedIndex)
        {
            // ARRANGE
            var indexList = new List<IHistogramBucket>();
            for (int i = 0; i < startIndexes.Length; i++)
                indexList.Add(new HistogramBucket(startIndexes[i], endIndexes[i], 1));

            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.GetMatchBucketIndex(indexList, lowerBound, upperBound, compareValue);

            // ASSERT
            Assert.AreEqual(expectedIndex, result);
        }

        #endregion

        #region GetBucketBounds

        [TestMethod]
        [DataRow(
            new int[] { 1, 11 },
            new int[] { 10, 20 },
            new int[] { 5, 31 },
            new int[] { 30, 40 },
            2,
            2)]
        [DataRow(
            new int[] { 1, 100 },
            new int[] { 10, 110 },
            new int[] { 5, 31 },
            new int[] { 30, 40 },
            0,
            0)]
        public void Can_GetBucketBounds_Less(int[] leftBucketsStart, int[] leftBucketsEnd, int[] rightBucketsStart, int[] rightBucketsEnd, int expLeftCount, int expRightCount)
        {
            // ARRANGE
            var bucketsLeft = new List<IHistogramBucket>();
            var rightLeft = new List<IHistogramBucket>();
            for (int i = 0; i < leftBucketsStart.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftBucketsStart[i], leftBucketsEnd[i], 1));
            for (int i = 0; i < rightBucketsStart.Length; i++)
                rightLeft.Add(new HistogramBucket(rightBucketsStart[i], rightBucketsEnd[i], 1));

            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.GetBucketBounds(ComparisonType.Type.Less, bucketsLeft, rightLeft);

            // ASSERT
            Assert.AreEqual(expLeftCount, result.LeftBuckets.Count);
            Assert.AreEqual(expRightCount, result.RightBuckets.Count);
        }

        [TestMethod]
        [DataRow(
            new int[] { 1, 11, 32 },
            new int[] { 10, 31, 200 },
            new int[] { 5, 31 },
            new int[] { 30, 40 },
            3,
            2)]
        [DataRow(
            new int[] { 1, 11 },
            new int[] { 10, 31 },
            new int[] { 1, 31, 41 },
            new int[] { 10, 40, 500 },
            2,
            3)]
        public void Can_GetBucketBounds_LessOrEqual(int[] leftBucketsStart, int[] leftBucketsEnd, int[] rightBucketsStart, int[] rightBucketsEnd, int expLeftCount, int expRightCount)
        {
            // ARRANGE
            var bucketsLeft = new List<IHistogramBucket>();
            var rightLeft = new List<IHistogramBucket>();
            for (int i = 0; i < leftBucketsStart.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftBucketsStart[i], leftBucketsEnd[i], 1));
            for (int i = 0; i < rightBucketsStart.Length; i++)
                rightLeft.Add(new HistogramBucket(rightBucketsStart[i], rightBucketsEnd[i], 1));

            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.GetBucketBounds(ComparisonType.Type.EqualOrLess, bucketsLeft, rightLeft);

            // ASSERT
            Assert.AreEqual(expLeftCount, result.LeftBuckets.Count);
            Assert.AreEqual(expRightCount, result.RightBuckets.Count);
        }

        [TestMethod]
        [DataRow(
            new int[] { 1, 11 },
            new int[] { 10, 20 },
            new int[] { 21, 31 },
            new int[] { 30, 40 },
            0,
            0)]
        [DataRow(
            new int[] { 1, 100 },
            new int[] { 10, 110 },
            new int[] { 5, 31 },
            new int[] { 30, 40 },
            2,
            2)]
        public void Can_GetBucketBounds_More(int[] leftBucketsStart, int[] leftBucketsEnd, int[] rightBucketsStart, int[] rightBucketsEnd, int expLeftCount, int expRightCount)
        {
            // ARRANGE
            var bucketsLeft = new List<IHistogramBucket>();
            var rightLeft = new List<IHistogramBucket>();
            for (int i = 0; i < leftBucketsStart.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftBucketsStart[i], leftBucketsEnd[i], 1));
            for (int i = 0; i < rightBucketsStart.Length; i++)
                rightLeft.Add(new HistogramBucket(rightBucketsStart[i], rightBucketsEnd[i], 1));

            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.GetBucketBounds(ComparisonType.Type.More, bucketsLeft, rightLeft);

            // ASSERT
            Assert.AreEqual(expLeftCount, result.LeftBuckets.Count);
            Assert.AreEqual(expRightCount, result.RightBuckets.Count);
        }

        [TestMethod]
        [DataRow(
            new int[] { 1, 11, 32 },
            new int[] { 10, 31, 200 },
            new int[] { 5, 31 },
            new int[] { 30, 40 },
            3,
            2)]
        [DataRow(
            new int[] { 1, 11 },
            new int[] { 10, 31 },
            new int[] { 1, 31, 41 },
            new int[] { 10, 40, 500 },
            2,
            2)]
        public void Can_GetBucketBounds_MoreOrEqual(int[] leftBucketsStart, int[] leftBucketsEnd, int[] rightBucketsStart, int[] rightBucketsEnd, int expLeftCount, int expRightCount)
        {
            // ARRANGE
            var bucketsLeft = new List<IHistogramBucket>();
            var rightLeft = new List<IHistogramBucket>();
            for (int i = 0; i < leftBucketsStart.Length; i++)
                bucketsLeft.Add(new HistogramBucket(leftBucketsStart[i], leftBucketsEnd[i], 1));
            for (int i = 0; i < rightBucketsStart.Length; i++)
                rightLeft.Add(new HistogramBucket(rightBucketsStart[i], rightBucketsEnd[i], 1));

            var estimator = new EstimateCalculatorEquiDepth(new HistogramManagerStub());

            // ACT
            var result = estimator.GetBucketBounds(ComparisonType.Type.EqualOrMore, bucketsLeft, rightLeft);

            // ASSERT
            Assert.AreEqual(expLeftCount, result.LeftBuckets.Count);
            Assert.AreEqual(expRightCount, result.RightBuckets.Count);
        }

        #endregion

        #region MakeNewIntermediateBucket

        [TestMethod]
        public void Can_MakeNewIntermediateBucket()
        {

        }

        #endregion
    }
}
