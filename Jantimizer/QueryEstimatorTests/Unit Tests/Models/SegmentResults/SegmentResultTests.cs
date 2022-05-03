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
    public class SegmentResultTests
    {
        [TestMethod]
        [DataRow(100, 1, 100)]
        [DataRow(100, 2, 200)]
        [DataRow(10, 10, 100)]
        public void Can_GetTotalEstimation(long countLeft, long countRight, long expCount)
        {
            // ARRANGE
            ISegmentResult resLeft = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), countLeft, ComparisonType.Type.Equal);
            ISegmentResult resRight = new ValueTableAttributeResult(new TableAttribute(), new TableAttribute(), countRight, ComparisonType.Type.Equal);

            // ACT
            ISegmentResult res = new SegmentResult(resLeft, resRight);

            // ASSERT
            Assert.AreEqual(res.GetTotalEstimation(), expCount);
        }

        [TestMethod]
        [DataRow("A", "v", "B", "v", "C", "v", "D", "v", "A", "v")]
        [DataRow("A", "v", "B", "v", "C", "v", "D", "v", "D", "v")]
        [DataRow("A", "v", "B", "v", "C", "v", "D", "v", "B", "v")]
        public void Can_DoesContainTableAttribute_True(string tableA, string attrA, string tableB, string attrB, string tableC, string attrC, string tableD, string attrD, string checkTable, string checkAttr)
        {
            // ARRANGE
            var tableAttrCheck = new TableAttribute(checkTable, checkAttr);
            ISegmentResult resLeft = new ValueTableAttributeResult(new TableAttribute(tableA, attrA), new TableAttribute(tableB, attrB), 1, ComparisonType.Type.Equal);
            ISegmentResult resRight = new ValueTableAttributeResult(new TableAttribute(tableC, attrC), new TableAttribute(tableD, attrD), 1, ComparisonType.Type.Equal);
            ISegmentResult res = new SegmentResult(resLeft, resRight);

            // ACT
            var result = res.DoesContainTableAttribute(tableAttrCheck);

            // ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("A", "v", "B", "v", "C", "v", "D", "v", "E", "v")]
        [DataRow("A", "v", "B", "v", "C", "v", "D", "v", "Q", "v")]
        [DataRow("A", "v", "B", "v", "C", "v", "D", "v", "F", "v")]
        public void Can_DoesContainTableAttribute_False(string tableA, string attrA, string tableB, string attrB, string tableC, string attrC, string tableD, string attrD, string checkTable, string checkAttr)
        {
            // ARRANGE
            var tableAttrCheck = new TableAttribute(checkTable, checkAttr);
            ISegmentResult resLeft = new ValueTableAttributeResult(new TableAttribute(tableA, attrA), new TableAttribute(tableB, attrB), 1, ComparisonType.Type.Equal);
            ISegmentResult resRight = new ValueTableAttributeResult(new TableAttribute(tableC, attrC), new TableAttribute(tableD, attrD), 1, ComparisonType.Type.Equal);
            ISegmentResult res = new SegmentResult(resLeft, resRight);

            // ACT
            var result = res.DoesContainTableAttribute(tableAttrCheck);

            // ASSERT
            Assert.IsFalse(result);
        }
    }
}
