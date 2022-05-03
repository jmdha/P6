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
            3,
            4)]
        public void Can_Bound_More(int[] sourceValues, int[] compareValues, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var sourceAttr = new TableAttribute("A", "v");
            var compareAttr = new TableAttribute("B", "v");

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            TestHistogramManager testManager = new TestHistogramManager();
            // Add A
            var sourceHistogram = new HistogramEquiDepth(sourceAttr.Table.TableName, sourceAttr.Attribute, 10);
            var sourceHistoValues = new List<ValueCount>();
            foreach(var value in sourceValues)
                sourceHistoValues.Add(new ValueCount(value, 10));
            sourceHistogram.GenerateSegmentationsFromSortedGroups(sourceHistoValues);
            testManager.AddHistogram(sourceHistogram);

            // Add B
            var compareHistogram = new HistogramEquiDepth(compareAttr.Table.TableName, compareAttr.Attribute, 10);
            var compareHistoValues = new List<ValueCount>();
            foreach (var value in compareValues)
                compareHistoValues.Add(new ValueCount(value, 10));
            compareHistogram.GenerateSegmentationsFromSortedGroups(compareHistoValues);
            testManager.AddHistogram(compareHistogram);

            //var segmentComparer = new SegmentationComparisonCalculator(DataGatherer);
            //segmentComparer.DoHistogramComparisons(testManager.Histograms.Values.ToList()).Wait();

            IPredicateBounder<TableAttribute> bounder = new TableAttributeBounder(upperBounds, lowerBounds, testManager);

            // ACT
            var result = bounder.Bound(sourceAttr, compareAttr, ComparisonType.Type.More);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        #endregion
    }
}
