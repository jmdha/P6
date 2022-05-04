using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryEstimator.PredicateBounders;
using QueryEstimatorTests.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Unit_Tests.PredicateBounders
{
    [TestClass]
    public class TableAttributeBounderTests
    {
        #region Bound

        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },
            new int[] { 40, 50, 60, 70, 80 },
            4,
            4)]
        [DataRow(
            new int[] { 10, 20, 30, 41, 50 },
            new int[] { 40, 50, 60, 70, 80 },
            2,
            4)]
        [DataRow(
            new int[] { 10, 40, 50, 60, 70 },
            new int[] { 40, 50, 60, 70, 80 },
            1,
            4)]
        [DataRow(
            new int[] { 10, 40, 50, 60, 70 },
            new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            0,
            4)]
        [DataRow(
            new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            new int[] { 10, 40, 50, 60, 70 },
            0,
            -1)]
        public void Can_Bound_More(int[] sourceValues, int[] compareValues, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttr = new TableAttribute("B", "v");

            IPredicateBounder<TableAttribute> bounder = GetBaseBounder(sourceAttr, sourceValues, compareAttr, compareValues);

            // ACT
            var result = bounder.Bound(sourceAttr, compareAttr, ComparisonType.Type.More);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },
            new int[] { 40, 50, 60, 70, 80 },
            0,
            4)]
                [DataRow(
            new int[] { 60, 65, 70, 80, 90 },
            new int[] { 10, 20, 30, 40, 66 },
            0,
            1)]
                [DataRow(
            new int[] { 60, 65, 70, 80, 90 },
            new int[] { 10, 20, 30, 40, 65 },
            0,
            0)]
                [DataRow(
            new int[] { 10, 40, 50, 60, 70 },
            new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            0,
            -1)]
                [DataRow(
            new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            new int[] { 10, 40, 50, 60, 70 },
            0,
            8)]
        public void Can_Bound_Less(int[] sourceValues, int[] compareValues, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttr = new TableAttribute("B", "v");

            IPredicateBounder<TableAttribute> bounder = GetBaseBounder(sourceAttr, sourceValues, compareAttr, compareValues);

            // ACT
            var result = bounder.Bound(sourceAttr, compareAttr, ComparisonType.Type.Less);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        [TestMethod]
        [DataRow(
            new int[] { 10, 20, 30, 40, 50 },
            new int[] { 40, 50, 60, 70, 80 },
            4,
            4)]
        [DataRow(
            new int[] { 60, 65, 70, 80, 90 },
            new int[] { 10, 20, 30, 40, 64 },
            0,
            0)]
        [DataRow(
            new int[] { 60, 65, 70, 80, 90 },
            new int[] { 59, 64, 69, 79, 89 },
            0,
            3)]
        [DataRow(
            new int[] { 10, 40, 50, 60, 70 },
            new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            0,
            -1)]
        [DataRow(
            new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 20 },
            new int[] { 10, 40, 50, 60, 70 },
            8,
            8)]
        public void Can_Bound_Equal(int[] sourceValues, int[] compareValues, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttr = new TableAttribute("B", "v");

            IPredicateBounder<TableAttribute> bounder = GetBaseBounder(sourceAttr, sourceValues, compareAttr, compareValues);

            // ACT
            var result = bounder.Bound(sourceAttr, compareAttr, ComparisonType.Type.Equal);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        #endregion

        #region Private Test Methods

        private IPredicateBounder<TableAttribute> GetBaseBounder(TableAttribute sourceAttr, int[] sourceValues, TableAttribute compareAttr, int[] compareValues)
        {
            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            TestHistogramManager testManager = new TestHistogramManager();
            TestDataGathereStub testGatherer = new TestDataGathereStub();

            // Add A table
            var sourceHistogram = new HistogramEquiDepth(sourceAttr.Table.TableName, sourceAttr.Attribute, 10);
            var sourceHistoValues = new List<ValueCount>();
            foreach (var value in sourceValues)
                sourceHistoValues.Add(new ValueCount(value, 10));
            var sourceAttributeData = new AttributeData(sourceAttr, sourceHistoValues, TypeCode.Int32);
            sourceHistogram.GenerateSegmentationsFromSortedGroups(sourceHistoValues);
            testManager.AddHistogram(sourceHistogram);

            // Add B table
            var compareHistogram = new HistogramEquiDepth(compareAttr.Table.TableName, compareAttr.Attribute, 10);
            var compareHistoValues = new List<ValueCount>();
            foreach (var value in compareValues)
                compareHistoValues.Add(new ValueCount(value, 10));
            var compareAttributeData = new AttributeData(compareAttr, compareHistoValues, TypeCode.Int32);
            compareHistogram.GenerateSegmentationsFromSortedGroups(compareHistoValues);
            testManager.AddHistogram(compareHistogram);

            testGatherer.Data.Add(sourceAttr, sourceAttributeData);
            testGatherer.Data.Add(compareAttr, compareAttributeData);

            var segmentComparer = new SegmentationComparisonCalculator(testGatherer);
            segmentComparer.DoHistogramComparisons(testManager.Histograms.Values.ToList()).Wait();

            return new TableAttributeBounder(upperBounds, lowerBounds, testManager);
        }

        #endregion
    }
}
