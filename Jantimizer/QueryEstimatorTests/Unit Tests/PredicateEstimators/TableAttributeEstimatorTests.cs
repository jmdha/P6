using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryEstimator.Models;
using QueryEstimator.PredicateEstimators;
using QueryEstimatorTests.Stubs;
using Milestoner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Unit_Tests.PredicateEstimators
{
    [TestClass]
    public class TableAttributeEstimatorTests
    {
        #region GetBoundedSegmentResult

        [TestMethod]
        [DataRow(1, 10, false, 0, 20, 10)]
        [DataRow(1, 10, true, 0, 20, 1)]
        [DataRow(2, 10, false, 0, 20, 20)]
        [DataRow(2, 10, true, 0, 20, 2)]
        [DataRow(2, 20, false, 0, 1000, 40)]
        [DataRow(2, 20, true, 0, 1000, 2)]
        public void Can_GetBoundedSegmentResult_NoBounding(long segmentCount, long addValue, bool doesPreviousContain, long bottomOffsetCount, long checkOffsetCount, long expected)
        {
            // ARRANGE
            TableAttributeEstimator bounder = GetBaseBounder();

            // ACT
            var result = bounder.GetBoundedSegmentResult(segmentCount, addValue, doesPreviousContain, bottomOffsetCount, checkOffsetCount);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(1, 10, false, 5, 20, 5)]
        [DataRow(1, 10, true, 5, 20, 1)]
        [DataRow(2, 10, false, 10, 20, 0)]
        [DataRow(2, 10, true, 10, 20, 0)]
        [DataRow(2, 20, false, 20, 1000, 0)]
        [DataRow(2, 20, true, 20, 1000, 0)]
        public void Can_GetBoundedSegmentResult_BottomBounding(long segmentCount, long addValue, bool doesPreviousContain, long bottomOffsetCount, long checkOffsetCount, long expected)
        {
            // ARRANGE
            TableAttributeEstimator bounder = GetBaseBounder();

            // ACT
            var result = bounder.GetBoundedSegmentResult(segmentCount, addValue, doesPreviousContain, bottomOffsetCount, checkOffsetCount);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(1, 10, false, 0, 10, 10)]
        [DataRow(1, 10, true, 0, 10, 1)]
        [DataRow(2, 10, false, 0, 5, 10)]
        [DataRow(2, 10, true, 0, 5, 2)]
        [DataRow(2, 20, false, 0, 20, 40)]
        [DataRow(2, 20, true, 0, 20, 2)]
        public void Can_GetBoundedSegmentResult_TopBounding(long segmentCount, long addValue, bool doesPreviousContain, long bottomOffsetCount, long checkOffsetCount, long expected)
        {
            // ARRANGE
            TableAttributeEstimator bounder = GetBaseBounder();

            // ACT
            var result = bounder.GetBoundedSegmentResult(segmentCount, addValue, doesPreviousContain, bottomOffsetCount, checkOffsetCount);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region GetEstimationResult (Unbounded, Single)

        // A.v < B.v
        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            1500)]                              // (50 * 10) + (40 * 10) + (30 * 10) + (20 * 10) + (10 * 10) = 1500
        [DataRow(
            new int[] { 40, 50, 60, 70, 80 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            300)]                               // (20 * 10) + (10 * 10) = 300
        [DataRow(
            new int[] { 70, 80, 90, 93, 99 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            0)]                                 // 0
        public void Can_GetEstimationResult_Single_Unbounded_Less(int[] sourceValues, int[] sourceCounts, int[] compareValues, int[] compareCounts, int expectedCount)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A","v");
            var compareAttr = new TableAttribute("B","v");

            TableAttributeEstimator bounder = GetSingleBounder(sourceAttr, sourceValues, sourceCounts, compareAttr, compareValues, compareCounts);
            ISegmentResult segmentResult = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), 1, ComparisonType.Type.None);

            // ACT
            var result = bounder.GetEstimationResult(segmentResult, sourceAttr, compareAttr, ComparisonType.Type.Less);

            // ASSERT
            Assert.AreEqual(expectedCount, result.GetTotalEstimation());
        }

        // A.v > B.v
        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            600)]                               // (10 * 10) + (20 * 10) + (30 * 10) = 600
        [DataRow(
            new int[] { 40, 50, 60, 70, 80 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            1900)]                              // (20 * 10) + (30 * 10) + (40 * 10) + (50 * 10) + (50 * 10) = 1900
        [DataRow(
            new int[] { 20, 30, 40, 50, 60 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 70, 80, 90, 93, 99 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            0)]                                 // 0
        public void Can_GetEstimationResult_Single_Unbounded_More(int[] sourceValues, int[] sourceCounts, int[] compareValues, int[] compareCounts, int expectedCount)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttr = new TableAttribute("B", "v");

            TableAttributeEstimator bounder = GetSingleBounder(sourceAttr, sourceValues, sourceCounts, compareAttr, compareValues, compareCounts);
            ISegmentResult segmentResult = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), 1, ComparisonType.Type.None);

            // ACT
            var result = bounder.GetEstimationResult(segmentResult, sourceAttr, compareAttr, ComparisonType.Type.More);

            // ASSERT
            Assert.AreEqual(expectedCount, result.GetTotalEstimation());
        }

        // A.v = B.v
        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            2500)]                              // Count(A.v) * Count(B.v) = 50 * 50 = 2500
        // Note, unbounded "=" always gives the full A.v count.
        public void Can_GetEstimationResult_Single_Unbounded_Equal(int[] sourceValues, int[] sourceCounts, int[] compareValues, int[] compareCounts, int expectedCount)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttr = new TableAttribute("B", "v");

            TableAttributeEstimator bounder = GetSingleBounder(sourceAttr, sourceValues, sourceCounts, compareAttr, compareValues, compareCounts);
            ISegmentResult segmentResult = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), 1, ComparisonType.Type.None);

            // ACT
            var result = bounder.GetEstimationResult(segmentResult, sourceAttr, compareAttr, ComparisonType.Type.Equal);

            // ASSERT
            Assert.AreEqual(expectedCount, result.GetTotalEstimation());
        }

        #endregion

        #region GetEstimationResult (Unbounded, Double)

        // A.v < B.v  then  B.v < C.v
        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            new int[] { 30, 40, 50, 60, 70 },   // C.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(C.v)
            75000)]                             // 1500      *  (10 + 10 + 10 + 10 + 10)
                                                // A.v < B.v          Count(C.v)
        [DataRow(
            new int[] { 40, 50, 60, 70, 80 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            new int[] { 20, 30, 40, 50, 60 },   // C.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(C.v)
            15000)]                             // 300         *  (10 + 10 + 10 + 10 + 10)
                                                // A.v < B.v            Count(C.v)
        public void Can_GetEstimationResult_Double_Unbounded_Less(int[] sourceValues, int[] sourceCounts, int[] compareValuesA, int[] compareCountsA, int[] compareValuesB, int[] compareCountsB, int expectedCount)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttrA = new TableAttribute("B", "v");
            var compareAttrB = new TableAttribute("C", "v");

            TableAttributeEstimator bounder = GetDoubleBounder(sourceAttr, sourceValues, sourceCounts, compareAttrA, compareValuesA, compareCountsA, compareAttrB, compareValuesB, compareCountsB);
            ISegmentResult segmentResult = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), 1, ComparisonType.Type.None);

            // ACT
            segmentResult = new SegmentResult(segmentResult, bounder.GetEstimationResult(segmentResult, sourceAttr, compareAttrA, ComparisonType.Type.Less));
            segmentResult = new SegmentResult(segmentResult, bounder.GetEstimationResult(segmentResult, compareAttrA, compareAttrB, ComparisonType.Type.Less));

            // ASSERT
            Assert.AreEqual(expectedCount, segmentResult.GetTotalEstimation());
        }

        // A.v > B.v  then  B.v > C.v
        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            new int[] { 30, 40, 50, 60, 70 },   // C.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(C.v)
            30000)]                             // 600        *  (10 + 10 + 10 + 10 + 10)
                                                // A.v > B.v          Count(C.v)
        [DataRow(
            new int[] { 40, 50, 60, 70, 80 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            new int[] { 20, 30, 40, 50, 60 },   // C.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(C.v)
            95000)]                             // 1900        *  (10 + 10 + 10 + 10 + 10)
                                                // A.v > B.v            Count(C.v)
        public void Can_GetEstimationResult_Double_Unbounded_More(int[] sourceValues, int[] sourceCounts, int[] compareValuesA, int[] compareCountsA, int[] compareValuesB, int[] compareCountsB, int expectedCount)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttrA = new TableAttribute("B", "v");
            var compareAttrB = new TableAttribute("C", "v");

            TableAttributeEstimator bounder = GetDoubleBounder(sourceAttr, sourceValues, sourceCounts, compareAttrA, compareValuesA, compareCountsA, compareAttrB, compareValuesB, compareCountsB);
            ISegmentResult segmentResult = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), 1, ComparisonType.Type.None);

            // ACT
            segmentResult = new SegmentResult(segmentResult, bounder.GetEstimationResult(segmentResult, sourceAttr, compareAttrA, ComparisonType.Type.More));
            segmentResult = new SegmentResult(segmentResult, bounder.GetEstimationResult(segmentResult, compareAttrA, compareAttrB, ComparisonType.Type.More));

            // ASSERT
            Assert.AreEqual(expectedCount, segmentResult.GetTotalEstimation());
        }

        // A.v = B.v  then  B.v = C.v
        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },   // A.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(A.v)
            new int[] { 20, 30, 40, 50, 60 },   // B.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(B.v)
            new int[] { 30, 40, 50, 60, 70 },   // C.v
            new int[] { 10, 10, 10, 10, 10 },   // Count(C.v)
            125000)]                            // 2500        *  (10 + 10 + 10 + 10 + 10)
                                                // A.v = B.v              Count(C.v)
        // Note, unbounded "=" always gives the full A.v count.
        public void Can_GetEstimationResult_Double_Unbounded_Equal(int[] sourceValues, int[] sourceCounts, int[] compareValuesA, int[] compareCountsA, int[] compareValuesB, int[] compareCountsB, int expectedCount)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttrA = new TableAttribute("B", "v");
            var compareAttrB = new TableAttribute("C", "v");

            TableAttributeEstimator bounder = GetDoubleBounder(sourceAttr, sourceValues, sourceCounts, compareAttrA, compareValuesA, compareCountsA, compareAttrB, compareValuesB, compareCountsB);
            ISegmentResult segmentResult = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), 1, ComparisonType.Type.None);

            // ACT
            segmentResult = new SegmentResult(segmentResult, bounder.GetEstimationResult(segmentResult, sourceAttr, compareAttrA, ComparisonType.Type.Equal));
            segmentResult = new SegmentResult(segmentResult, bounder.GetEstimationResult(segmentResult, compareAttrA, compareAttrB, ComparisonType.Type.Equal));

            // ASSERT
            Assert.AreEqual(expectedCount, segmentResult.GetTotalEstimation());
        }

        #endregion

        #region Private Test Methods

        private TableAttributeEstimator GetBaseBounder()
        {
            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            return new TableAttributeEstimator(upperBounds, lowerBounds, new TestMilestonerManager());
        }

        private TableAttributeEstimator GetSingleBounder(TableAttribute sourceAttr, int[] sourceValues, int[] sourceCounts, TableAttribute compareAttr, int[] compareValues, int[] compareCounts)
        {
            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            TestMilestonerManager testManager = new TestMilestonerManager();

            // Add A table
            var sourceHistoValues = new List<ValueCount>();
            for(int i = 0; i < sourceValues.Length; i++)
                sourceHistoValues.Add(new ValueCount(sourceValues[i], sourceCounts[i]));
            testManager.AddMilestonesFromValueCount(sourceAttr, sourceHistoValues);

            // Add B table
            var compareHistoValues = new List<ValueCount>();
            for (int i = 0; i < compareValues.Length; i++)
                compareHistoValues.Add(new ValueCount(compareValues[i], compareCounts[i]));
            testManager.AddMilestonesFromValueCount(compareAttr, compareHistoValues);

            testManager.Comparer.DoMilestoneComparisons();

            return new TableAttributeEstimator(upperBounds, lowerBounds, testManager);
        }

        private TableAttributeEstimator GetDoubleBounder(TableAttribute sourceAttr, int[] sourceValues, int[] sourceCounts, TableAttribute compareAttrA, int[] compareValuesA, int[] compareCountsA, TableAttribute compareAttrB, int[] compareValuesB, int[] compareCountsB)
        {
            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            TestMilestonerManager testManager = new TestMilestonerManager();

            // Add A table
            var sourceHistoValues = new List<ValueCount>();
            for (int i = 0; i < sourceValues.Length; i++)
                sourceHistoValues.Add(new ValueCount(sourceValues[i], sourceCounts[i]));
            testManager.AddMilestonesFromValueCount(sourceAttr, sourceHistoValues);

            // Add Compare A table
            var compareAHistoValues = new List<ValueCount>();
            for (int i = 0; i < compareValuesA.Length; i++)
                compareAHistoValues.Add(new ValueCount(compareValuesA[i], compareCountsA[i]));
            testManager.AddMilestonesFromValueCount(compareAttrA, compareAHistoValues);

            // Add Compare B table
            var compareBHistoValues = new List<ValueCount>();
            for (int i = 0; i < compareValuesB.Length; i++)
                compareBHistoValues.Add(new ValueCount(compareValuesB[i], compareCountsB[i]));
            testManager.AddMilestonesFromValueCount(compareAttrB, compareBHistoValues);

            testManager.Comparer.DoMilestoneComparisons();

            return new TableAttributeEstimator(upperBounds, lowerBounds, testManager);
        }

        #endregion
    }
}
