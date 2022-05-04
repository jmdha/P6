using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryEstimator.PredicateBounders;
using QueryEstimatorTests.Stubs;
using Segmentator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Unit_Tests.PredicateBounders
{
    [TestClass]
    public class SimpleFilterBounderTests
    {
        #region Bound

        [TestMethod]
        [DataRow(19, 2, 5)]
        [DataRow(20, 3, 5)]
        [DataRow(21, 3, 5)]

        [DataRow(9, 1, 5)]
        [DataRow(10, 2, 5)]
        [DataRow(11, 2, 5)]

        [DataRow(0, 1, 5)]
        [DataRow(1, 1, 5)]

        [DataRow(49, 5, 5)]
        [DataRow(50, 5, 5)]
        [DataRow(51, 5, 5)]
        public void Can_Bound_More(int compareTo, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var tableAttr = new TableAttribute("A", "v");
            IPredicateBounder<IComparable> bounder = GetBaseBounder(tableAttr);

            // ACT
            var result = bounder.Bound(tableAttr, compareTo, ComparisonType.Type.More);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        [TestMethod]
        [DataRow(19, 2, 5)]
        [DataRow(20, 2, 5)]
        [DataRow(21, 3, 5)]

        [DataRow(9, 1, 5)]
        [DataRow(10, 1, 5)]
        [DataRow(11, 2, 5)]

        [DataRow(0, 0, 5)]
        [DataRow(1, 1, 5)]

        [DataRow(49, 5, 5)]
        [DataRow(50, 5, 5)]
        [DataRow(51, 5, 5)]
        public void Can_Bound_MoreOrEqual(int compareTo, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var tableAttr = new TableAttribute("A", "v");
            IPredicateBounder<IComparable> bounder = GetBaseBounder(tableAttr);

            // ACT
            var result = bounder.Bound(tableAttr, compareTo, ComparisonType.Type.EqualOrMore);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        [TestMethod]
        [DataRow(19, 0, 1)]
        [DataRow(20, 0, 1)]
        [DataRow(21, 0, 2)]

        [DataRow(9, 0, 0)]
        [DataRow(10, 0, 0)]
        [DataRow(11, 0, 1)]

        [DataRow(0, 0, -1)]
        [DataRow(1, 0, 0)]

        [DataRow(49, 0, 4)]
        [DataRow(50, 0, 4)]
        [DataRow(51, 0, 5)]
        public void Can_Bound_Less(int compareTo, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var tableAttr = new TableAttribute("A", "v");
            IPredicateBounder<IComparable> bounder = GetBaseBounder(tableAttr);

            // ACT
            var result = bounder.Bound(tableAttr, compareTo, ComparisonType.Type.Less);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        [TestMethod]
        [DataRow(19, 0, 1)]
        [DataRow(20, 0, 2)]
        [DataRow(21, 0, 2)]

        [DataRow(9, 0, 0)]
        [DataRow(10, 0, 1)]
        [DataRow(11, 0, 1)]

        [DataRow(0, 0, 0)]
        [DataRow(1, 0, 0)]

        [DataRow(49, 0, 4)]
        [DataRow(50, 0, 5)]
        [DataRow(51, 0, 5)]
        public void Can_Bound_LessOrEqual(int compareTo, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var tableAttr = new TableAttribute("A", "v");
            IPredicateBounder<IComparable> bounder = GetBaseBounder(tableAttr);

            // ACT
            var result = bounder.Bound(tableAttr, compareTo, ComparisonType.Type.EqualOrLess);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        [TestMethod]
        [DataRow(19, 1, 1)]
        [DataRow(20, 2, 2)]
        [DataRow(21, 2, 2)]

        [DataRow(9, 0, 0)]
        [DataRow(10, 1, 1)]
        [DataRow(11, 1, 1)]

        [DataRow(0, 0, 0)]
        [DataRow(1, 0, 0)]

        [DataRow(49, 4, 4)]
        [DataRow(50, 5, 5)]
        [DataRow(51, 5, 5)]
        public void Can_Bound_Equal_1(int compareTo, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var tableAttr = new TableAttribute("A", "v");
            IPredicateBounder<IComparable> bounder = GetBaseBounder(tableAttr);

            // ACT
            var result = bounder.Bound(tableAttr, compareTo, ComparisonType.Type.Equal);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        [TestMethod]
        [DataRow(19, 1, 1)]
        [DataRow(20, 2, 4)]
        [DataRow(21, 4, 4)]
        public void Can_Bound_Equal_2(int compareTo, int expLowerBound, int expUpperBound)
        {
            // ARRANGE
            var tableAttr = new TableAttribute("A", "v");

            var overrideValues = new List<ValueCount>();
            overrideValues.Add(new ValueCount(0, 10));
            overrideValues.Add(new ValueCount(10, 10));
            overrideValues.Add(new ValueCount(20, 10));
            overrideValues.Add(new ValueCount(20, 10));
            overrideValues.Add(new ValueCount(20, 10));
            overrideValues.Add(new ValueCount(30, 10));
            overrideValues.Add(new ValueCount(40, 10));
            overrideValues.Add(new ValueCount(50, 10));
            IPredicateBounder<IComparable> bounder = GetBaseBounder(tableAttr, overrideValues);

            // ACT
            var result = bounder.Bound(tableAttr, compareTo, ComparisonType.Type.Equal);

            // ASSERT
            Assert.AreEqual(expLowerBound, result.LowerBound);
            Assert.AreEqual(expUpperBound, result.UpperBound);
        }

        #endregion

        #region ConvertCompareTypes

        [TestMethod]
        [DataRow(5, "5")]
        [DataRow(5.51, "5.51")]
        [DataRow("abc", "abc")]
        public void Can_ConvertCompareTypes(IComparable expItem, IComparable item)
        {
            // ARRANGE
            var tableAttr = new TableAttribute("A", "v");

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            TestMilestonerManager testManager = new TestMilestonerManager();
            var histoValues = new List<ValueCount>();
            histoValues.Add(new ValueCount(expItem, 10));
            testManager.AddMilestonesFromValueCount(tableAttr, histoValues);
            SimpleFilterBounder bounder = new SimpleFilterBounder(upperBounds, lowerBounds, testManager);

            // ACT
            var result = bounder.ConvertCompareTypes(bounder.GetAllMilestonesForAttribute(tableAttr)[0], item);

            // ASSERT
            Assert.AreEqual(expItem, result);
        }

        #endregion

        #region Private Test Methods

        private IPredicateBounder<IComparable> GetBaseBounder(TableAttribute tableAttr, List<ValueCount>? overrideList = null)
        {
            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            TestMilestonerManager testManager = new TestMilestonerManager();
            if (overrideList == null)
            {
                var histoValues = new List<ValueCount>();
                histoValues.Add(new ValueCount(0, 10));
                histoValues.Add(new ValueCount(10, 10));
                histoValues.Add(new ValueCount(20, 10));
                histoValues.Add(new ValueCount(30, 10));
                histoValues.Add(new ValueCount(40, 10));
                histoValues.Add(new ValueCount(50, 10));
                testManager.AddMilestonesFromValueCount(tableAttr, histoValues);
            }
            else
                testManager.AddMilestonesFromValueCount(tableAttr, overrideList);

            return new SimpleFilterBounder(upperBounds, lowerBounds, testManager);
        }

        #endregion
    }
}
