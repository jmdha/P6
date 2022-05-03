using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryEstimator.SegmentHandler;
using QueryEstimatorTests.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Unit_Tests.SegmentHandlers
{
    [TestClass]
    public class BaseSegmentBoundsHandlerTests
    {
        #region AddOrReduceUpperBound

        [TestMethod]
        [DataRow(5)]
        [DataRow(1000)]
        [DataRow(1)]
        public void Can_AddOrReduceUpperBound_Add(int value)
        {
            // ARRANGE
            var key = new TableAttribute();

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());

            // ACT
            boundsHandler.AddOrReduceUpperBound(key, value);

            // ASSERT
            Assert.AreEqual(upperBounds[key], value);
        }

        [TestMethod]
        [DataRow(5, 4)]
        [DataRow(1000, 1)]
        [DataRow(1, 0)]
        public void Can_AddOrReduceUpperBound_Reduce(int initialValue, int newValue)
        {
            // ARRANGE
            var key = new TableAttribute();

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());
            boundsHandler.AddOrReduceUpperBound(key, initialValue);

            // ACT
            boundsHandler.AddOrReduceUpperBound(key, newValue);

            // ASSERT
            Assert.AreEqual(upperBounds[key], newValue);
        }

        [TestMethod]
        [DataRow(5, 6)]
        [DataRow(1000, 108582)]
        [DataRow(1, 2)]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Cant_AddOrReduceUpperBound_IfHigher(int initialValue, int newValue)
        {
            // ARRANGE
            var key = new TableAttribute();

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());
            boundsHandler.AddOrReduceUpperBound(key, initialValue);

            // ACT
            boundsHandler.AddOrReduceUpperBound(key, newValue);
        }

        #endregion

        #region AddOrReduceLowerBound

        [TestMethod]
        [DataRow(5)]
        [DataRow(1000)]
        [DataRow(1)]
        public void Can_AddOrReduceLowerBound_Add(int value)
        {
            // ARRANGE
            var key = new TableAttribute();

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());

            // ACT
            boundsHandler.AddOrReduceLowerBound(key, value);

            // ASSERT
            Assert.AreEqual(lowerBounds[key], value);
        }

        [TestMethod]
        [DataRow(5, 6)]
        [DataRow(1000, 1573)]
        [DataRow(1, 2)]
        public void Can_AddOrReduceLowerBound_Reduce(int initialValue, int newValue)
        {
            // ARRANGE
            var key = new TableAttribute();

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());
            boundsHandler.AddOrReduceLowerBound(key, initialValue);

            // ACT
            boundsHandler.AddOrReduceLowerBound(key, newValue);

            // ASSERT
            Assert.AreEqual(lowerBounds[key], newValue);
        }

        [TestMethod]
        [DataRow(5, 4)]
        [DataRow(1000, 1)]
        [DataRow(1, 0)]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Cant_AddOrReduceLowerBound_IfLower(int initialValue, int newValue)
        {
            // ARRANGE
            var key = new TableAttribute();

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());
            boundsHandler.AddOrReduceLowerBound(key, initialValue);

            // ACT
            boundsHandler.AddOrReduceLowerBound(key, newValue);
        }

        #endregion

        #region GetUpperBoundOrAlt

        [TestMethod]
        [DataRow("A", "v", "A", "v", 5, 1)]
        [DataRow("B", "v", "B", "v", 1653, 1)]
        [DataRow("C", "v", "C", "v", 6565, 1)]
        public void GetUpperBoundOrAlt_Actual(string addTable, string addAttr, string checkTable, string checkAttr, int value, int altValue)
        {
            // ARRANGE
            var addKey = new TableAttribute(addTable, addAttr);
            var checkKey = new TableAttribute(checkTable, checkAttr);

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());
            boundsHandler.AddOrReduceUpperBound(addKey, value);

            // ACT
            var res = boundsHandler.GetUpperBoundOrAlt(checkKey, altValue);

            // ASSERT
            Assert.AreEqual(value, res);
        }

        [TestMethod]
        [DataRow("A", "v", "B", "v", 5, 166)]
        [DataRow("B", "v", "C", "v", 1653, 124)]
        [DataRow("C", "v", "A", "v", 6565, 11)]
        public void GetUpperBoundOrAlt_Alt(string addTable, string addAttr, string checkTable, string checkAttr, int value, int altValue)
        {
            // ARRANGE
            var addKey = new TableAttribute(addTable, addAttr);
            var checkKey = new TableAttribute(checkTable, checkAttr);

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());
            boundsHandler.AddOrReduceUpperBound(addKey, value);

            // ACT
            var res = boundsHandler.GetUpperBoundOrAlt(checkKey, altValue);

            // ASSERT
            Assert.AreEqual(altValue, res);
        }

        #endregion

        #region GetLowerBoundOrAlt

        [TestMethod]
        [DataRow("A", "v", "A", "v", 5, 1)]
        [DataRow("B", "v", "B", "v", 1653, 1)]
        [DataRow("C", "v", "C", "v", 6565, 1)]
        public void GetLowerBoundOrAlt_Actual(string addTable, string addAttr, string checkTable, string checkAttr, int value, int altValue)
        {
            // ARRANGE
            var addKey = new TableAttribute(addTable, addAttr);
            var checkKey = new TableAttribute(checkTable, checkAttr);

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());
            boundsHandler.AddOrReduceUpperBound(addKey, value);

            // ACT
            var res = boundsHandler.GetUpperBoundOrAlt(checkKey, altValue);

            // ASSERT
            Assert.AreEqual(value, res);
        }

        [TestMethod]
        [DataRow("A", "v", "B", "v", 5, 166)]
        [DataRow("B", "v", "C", "v", 1653, 124)]
        [DataRow("C", "v", "A", "v", 6565, 11)]
        public void GetLowerBoundOrAlt_Alt(string addTable, string addAttr, string checkTable, string checkAttr, int value, int altValue)
        {
            // ARRANGE
            var addKey = new TableAttribute(addTable, addAttr);
            var checkKey = new TableAttribute(checkTable, checkAttr);

            Dictionary<TableAttribute, int> upperBounds = new Dictionary<TableAttribute, int>();
            Dictionary<TableAttribute, int> lowerBounds = new Dictionary<TableAttribute, int>();
            ISegmentHandler boundsHandler = new TestSegmentBoundsHandler(upperBounds, lowerBounds, new TestHistogramManager());
            boundsHandler.AddOrReduceLowerBound(addKey, value);

            // ACT
            var res = boundsHandler.GetLowerBoundOrAlt(checkKey, altValue);

            // ASSERT
            Assert.AreEqual(altValue, res);
        }

        #endregion
    }
}
