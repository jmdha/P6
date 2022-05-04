using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryEstimator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Unit_Tests.Models.SegmentResults
{
    [TestClass]
    public class ValueTableAttributeResultTests
    {
        [TestMethod]
        [DataRow(100)]
        [DataRow(1)]
        [DataRow(155783957)]
        public void Can_GetTotalEstimation(long count)
        {
            // ARRANGE
            // ACT
            ISegmentResult res = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), count, ComparisonType.Type.Equal);

            // ASSERT
            Assert.AreEqual(res.GetTotalEstimation(), count);
        }

        [TestMethod]
        [DataRow("A", "v", "B", "v", "A", "v")]
        [DataRow("A", "v", "B", "v", "B", "v")]
        [DataRow("A", "v", "C", "C", "C", "C")]
        public void Can_DoesContainTableAttribute_True(string tableA, string attrA, string tableB, string attrB, string checkTable, string checkAttr)
        {
            // ARRANGE
            var tableAttrCheck = new TableAttribute(checkTable, checkAttr);

            var tableAttrA = new TableAttribute(tableA, attrA);
            var tableAttrB = new TableAttribute(tableB, attrB);
            ISegmentResult res = new ValueTableAttributeResult(tableAttrA, tableAttrB, 1, ComparisonType.Type.Equal);

            // ACT
            var result = res.DoesContainTableAttribute(tableAttrCheck);

            // ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("A", "v", "B", "v", "C", "v")]
        [DataRow("A", "v", "B", "v", "D", "v")]
        [DataRow("A", "v", "C", "C", "Q", "C")]
        public void Can_DoesContainTableAttribute_False(string tableA, string attrA, string tableB, string attrB, string checkTable, string checkAttr)
        {
            // ARRANGE
            var tableAttrCheck = new TableAttribute(checkTable, checkAttr);

            var tableAttrA = new TableAttribute(tableA, attrA);
            var tableAttrB = new TableAttribute(tableB, attrB);
            ISegmentResult res = new ValueTableAttributeResult(tableAttrA, tableAttrB, 1, ComparisonType.Type.Equal);

            // ACT
            var result = res.DoesContainTableAttribute(tableAttrCheck);

            // ASSERT
            Assert.IsFalse(result);
        }
    }
}
