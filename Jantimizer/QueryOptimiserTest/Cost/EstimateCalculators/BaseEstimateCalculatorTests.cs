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
    public class BaseEstimateCalculatorTests
    {
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
                Assert.AreEqual(expTables[i], result[i].Name);
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
            var tableAttribute1 = new TableAttribute("a", "v");
            var tableAttribute2 = new TableAttribute("b", "v");
            var tableAttribute3 = new TableAttribute("c", "v");
            var tableAttribute4 = new TableAttribute("d", "v");

            var tableRefNode1 = new TableReferenceNode(0, tableAttribute1.Name, tableAttribute1.Name);
            var tableRefNode2 = new TableReferenceNode(1, tableAttribute2.Name, tableAttribute2.Name);
            var tableRefNode3 = new TableReferenceNode(2, tableAttribute3.Name, tableAttribute3.Name);
            var tableRefNode4 = new TableReferenceNode(3, tableAttribute4.Name, tableAttribute4.Name);

            IHistogram testHistogram1 = new HistogramEquiDepth(tableAttribute1.Name, tableAttribute1.Attribute, 10);
            IHistogramBucket testHistogram1Bucket1 = new HistogramBucket(0, 10, 10);
            IHistogramBucket testHistogram1Bucket2 = new HistogramBucket(11, 20, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAttribute2.Name, tableAttribute2.Attribute, 10);
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
            tableBucket1.AddBucketIfNotThere(tableAttribute1, new BucketEstimate(tableHistogramBucket1, 1));
            IntermediateBucket tableBucket2 = new IntermediateBucket();
            tableBucket2.AddBucketIfNotThere(tableAttribute2, new BucketEstimate(tableHistogramBucket2, 1));

            var table = new IntermediateTable();
            table.Buckets.Add(tableBucket1);
            table.Buckets.Add(tableBucket2);
            table.References.Add(tableAttribute3);
            table.References.Add(tableAttribute4);

            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketPair(tableRefNode1, tableAttribute1.Attribute, tableRefNode2, tableAttribute2.Attribute, table);

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
            var tableAttribute1 = new TableAttribute("a", "v");
            var tableAttribute2 = new TableAttribute("b", "v");
            var tableAttribute3 = new TableAttribute("a", "v");
            var tableAttribute4 = new TableAttribute("d", "v");

            var tableRefNode1 = new TableReferenceNode(0, tableAttribute1.Name, tableAttribute1.Name);
            var tableRefNode2 = new TableReferenceNode(1, tableAttribute2.Name, tableAttribute2.Name);
            var tableRefNode3 = new TableReferenceNode(2, tableAttribute3.Name, tableAttribute3.Name);
            var tableRefNode4 = new TableReferenceNode(3, tableAttribute4.Name, tableAttribute4.Name);

            IHistogram testHistogram1 = new HistogramEquiDepth(tableAttribute1.Name, tableAttribute1.Attribute, 10);
            IHistogramBucket testHistogram1Bucket1 = new HistogramBucket(0, 10, 10);
            IHistogramBucket testHistogram1Bucket2 = new HistogramBucket(11, 20, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAttribute2.Name, tableAttribute2.Attribute, 10);
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
            tableBucket1.AddBucketIfNotThere(tableAttribute3, new BucketEstimate(tableHistogramBucket1, 1));
            IntermediateBucket tableBucket2 = new IntermediateBucket();
            tableBucket2.AddBucketIfNotThere(tableAttribute4, new BucketEstimate(tableHistogramBucket2, 1));

            var table = new IntermediateTable();
            table.Buckets.Add(tableBucket1);
            table.Buckets.Add(tableBucket2);
            table.References.Add(tableAttribute3);
            table.References.Add(tableAttribute4);

            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketPair(tableRefNode1, tableAttribute1.Attribute, tableRefNode2, tableAttribute2.Attribute, table);

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
            var tableAttribute1 = new TableAttribute("a", "v");
            var tableAttribute2 = new TableAttribute("b", "v");
            var tableAttribute3 = new TableAttribute("c", "v");
            var tableAttribute4 = new TableAttribute("b", "v");

            var tableRefNode1 = new TableReferenceNode(0, tableAttribute1.Name, tableAttribute1.Name);
            var tableRefNode2 = new TableReferenceNode(1, tableAttribute2.Name, tableAttribute2.Name);
            var tableRefNode3 = new TableReferenceNode(2, tableAttribute3.Name, tableAttribute3.Name);
            var tableRefNode4 = new TableReferenceNode(3, tableAttribute4.Name, tableAttribute4.Name);

            IHistogram testHistogram1 = new HistogramEquiDepth(tableAttribute1.Name, tableAttribute1.Attribute, 10);
            IHistogramBucket testHistogram1Bucket1 = new HistogramBucket(0, 10, 10);
            IHistogramBucket testHistogram1Bucket2 = new HistogramBucket(11, 20, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAttribute2.Name, tableAttribute2.Attribute, 10);
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
            tableBucket1.AddBucketIfNotThere(tableAttribute3, new BucketEstimate(tableHistogramBucket1, 1));
            IntermediateBucket tableBucket2 = new IntermediateBucket();
            tableBucket2.AddBucketIfNotThere(tableAttribute4, new BucketEstimate(tableHistogramBucket2, 1));

            var table = new IntermediateTable();
            table.Buckets.Add(tableBucket1);
            table.Buckets.Add(tableBucket2);
            table.References.Add(tableAttribute3);
            table.References.Add(tableAttribute4);

            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketPair(tableRefNode1, tableAttribute1.Attribute, tableRefNode2, tableAttribute2.Attribute, table);

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
            var tableAttribute1 = new TableAttribute("a", "v");
            var tableAttribute2 = new TableAttribute("b", "v");
            var tableAttribute3 = new TableAttribute("a", "v");
            var tableAttribute4 = new TableAttribute("b", "v");

            var tableRefNode1 = new TableReferenceNode(0, tableAttribute1.Name, tableAttribute1.Name);
            var tableRefNode2 = new TableReferenceNode(1, tableAttribute2.Name, tableAttribute2.Name);
            var tableRefNode3 = new TableReferenceNode(2, tableAttribute3.Name, tableAttribute3.Name);
            var tableRefNode4 = new TableReferenceNode(3, tableAttribute4.Name, tableAttribute4.Name);

            IHistogram testHistogram1 = new HistogramEquiDepth(tableAttribute1.Name, tableAttribute1.Attribute, 10);
            IHistogramBucket testHistogram1Bucket1 = new HistogramBucket(0, 10, 10);
            IHistogramBucket testHistogram1Bucket2 = new HistogramBucket(11, 20, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAttribute2.Name, tableAttribute2.Attribute, 10);
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
            tableBucket1.AddBucketIfNotThere(tableAttribute3, new BucketEstimate(tableHistogramBucket1, 1));
            IntermediateBucket tableBucket2 = new IntermediateBucket();
            tableBucket2.AddBucketIfNotThere(tableAttribute4, new BucketEstimate(tableHistogramBucket2, 1));

            var table = new IntermediateTable();
            table.Buckets.Add(tableBucket1);
            table.Buckets.Add(tableBucket2);
            table.References.Add(tableAttribute3);
            table.References.Add(tableAttribute4);

            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketPair(tableRefNode1, tableAttribute1.Attribute, tableRefNode2, tableAttribute2.Attribute, table);

            // ASSERT
            Assert.AreEqual(1, result.LeftBuckets.Count);
            Assert.AreEqual(1, result.RightBuckets.Count);
            Assert.IsTrue(result.LeftBuckets.Contains(tableHistogramBucket1));
            Assert.IsTrue(result.RightBuckets.Contains(tableHistogramBucket2));
        }

        #endregion
        
        #region GetBucketMatchesFromPredicate

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_Equal_1()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(10, 20, 10),
                new HistogramBucket(21, 40, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.Equal);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(2, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_Equal_2()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.Equal);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(2, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_More_1()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(5, 13, 10),
                new HistogramBucket(14, 30, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.More);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(3, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_More_2()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(12, 13, 10),
                new HistogramBucket(14, 30, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.More);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(2, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_MoreOrEqual_1()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(10, 13, 10),
                new HistogramBucket(14, 30, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.EqualOrMore);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(3, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_MoreOrEqual_2()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(20, 25, 10),
                new HistogramBucket(23, 40, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.EqualOrMore);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(1, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_Less_1()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(20, 25, 10),
                new HistogramBucket(23, 40, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.Less);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(4, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_Less_2()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(10, 30, 10),
                new HistogramBucket(31, 40, 10),
                new HistogramBucket(20, 25, 10),
                new HistogramBucket(23, 40, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.Less);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(2, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_LessOrEqual_1()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(0, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(0, 1, 10),
                new HistogramBucket(2, 20, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.EqualOrLess);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(2, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_GetBucketMatchesFromPredicate_LessOrEqual_2()
        {
            // ARRANGE
            var tableAtt1 = new TableAttribute("a", "v");
            var tableAtt2 = new TableAttribute("b", "v");

            HistogramManagerStub manager = MakeBasicHistogramManager(
                tableAtt1,
                tableAtt2,
                new HistogramBucket(5, 10, 10),
                new HistogramBucket(11, 20, 10),
                new HistogramBucket(0, 1, 10),
                new HistogramBucket(2, 3, 10));

            JoinPredicate predicate = MakeBasicJoinPredicate(
                tableAtt1,
                tableAtt2,
                ComparisonType.Type.EqualOrLess);

            var table = new IntermediateTable();
            var estimator = new EstimateCalculatorEquiDepth(manager);

            // ACT
            var result = estimator.GetBucketMatchesFromPredicate(predicate, table);

            // ASSERT
            Assert.AreEqual(0, result.References.Count);
            Assert.AreEqual(0, result.Buckets.Count);
        }

        #endregion

        #region Private Test Methods

        private HistogramManagerStub MakeBasicHistogramManager(TableAttribute tableAtt1, TableAttribute tableAtt2, IHistogramBucket testHistogram1Bucket1, IHistogramBucket testHistogram1Bucket2, IHistogramBucket testHistogram2Bucket1, IHistogramBucket testHistogram2Bucket2)
        {
            IHistogram testHistogram1 = new HistogramEquiDepth(tableAtt1.Name, tableAtt1.Attribute, 10);
            testHistogram1.Buckets.Add(testHistogram1Bucket1);
            testHistogram1.Buckets.Add(testHistogram1Bucket2);
            IHistogram testHistogram2 = new HistogramEquiDepth(tableAtt2.Name, tableAtt2.Attribute, 10);
            testHistogram2.Buckets.Add(testHistogram2Bucket1);
            testHistogram2.Buckets.Add(testHistogram2Bucket2);

            HistogramManagerStub manager = new HistogramManagerStub();
            manager.TestStorage.Add(testHistogram1);
            manager.TestStorage.Add(testHistogram2);

            return manager;
        }

        private JoinPredicate MakeBasicJoinPredicate(TableAttribute tableAtt1, TableAttribute tableAtt2, ComparisonType.Type compType)
        {
            return new JoinPredicate(
                new TableReferenceNode(0, tableAtt1.Name, tableAtt1.Name),
                tableAtt1.Attribute,
                new TableReferenceNode(1, tableAtt2.Name, tableAtt2.Name),
                tableAtt2.Attribute,
                $"{tableAtt1.Name}.{tableAtt1.Attribute} {ComparisonType.GetOperatorString(compType)} {tableAtt2.Name}.{tableAtt2.Attribute}",
                compType
                );
        }

        #endregion
    }
}
