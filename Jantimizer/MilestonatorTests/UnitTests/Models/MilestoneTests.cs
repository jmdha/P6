using Microsoft.VisualStudio.TestTools.UnitTesting;
using Milestoner.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace MilestonatorTests.UnitTests.Models
{
    [TestClass]
    public class MilestoneTests
    {
        [TestMethod]
        [DataRow(1,1)]
        [DataRow(1.5,10)]
        [DataRow(-40,200)]
        public void Can_SetProperties(IComparable lowestValue, long elements)
        {
            // ARRANGE
            // ACT
            IMilestone milestone = new Milestone(lowestValue, elements);

            // ASSERT
            Assert.AreEqual(lowestValue, milestone.LowestValue);
            Assert.AreEqual(elements, milestone.ElementsBeforeNextSegmentation);
        }

        [TestMethod]
        [DataRow("A", "a", "V", 10UL)]
        [DataRow("B", "someweirdAlias", "v", 5000UL)]
        public void Can_GetCountSmallerThanNoAlias(string tableName, string alias, string attribute, ulong expCount)
        {
            // ARRANGE
            var attrOrg = new TableAttribute(new TableReferenceNode(tableName, alias), attribute);

            IMilestone milestone = new Milestone(1, 1);
            milestone.CountSmallerThan.Add(new TableAttribute(tableName, attribute), expCount);

            // ACT
            var result = milestone.GetCountSmallerThanNoAlias(attrOrg);

            // ASSERT
            Assert.AreEqual(expCount, result);
        }

        [TestMethod]
        [DataRow("A", "a", "V", 10UL)]
        [DataRow("B", "someweirdAlias", "v", 5000UL)]
        public void Can_GetCountLargerThanNoAlias(string tableName, string alias, string attribute, ulong expCount)
        {
            // ARRANGE
            var attrOrg = new TableAttribute(new TableReferenceNode(tableName, alias), attribute);

            IMilestone milestone = new Milestone(1, 1);
            milestone.CountLargerThan.Add(new TableAttribute(tableName, attribute), expCount);

            // ACT
            var result = milestone.GetCountLargerThanNoAlias(attrOrg);

            // ASSERT
            Assert.AreEqual(expCount, result);
        }

        #region IsAnySmallerThanNoAlias

        [TestMethod]
        [DataRow("A", "a", "V")]
        [DataRow("B", "someweirdAlias", "v")]
        public void Can_IsAnySmallerThanNoAlias_True(string tableName, string alias, string attribute)
        {
            // ARRANGE
            var attrOrg = new TableAttribute(new TableReferenceNode(tableName, alias), attribute);

            IMilestone milestone = new Milestone(1, 1);
            milestone.CountSmallerThan.Add(new TableAttribute(tableName, attribute), 10UL);

            // ACT
            var result = milestone.IsAnySmallerThanNoAlias(attrOrg);

            // ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("A", "a", "V")]
        [DataRow("B", "someweirdAlias", "v")]
        public void Can_IsAnySmallerThanNoAlias_False(string tableName, string alias, string attribute)
        {
            // ARRANGE
            var attrOrg = new TableAttribute(new TableReferenceNode(tableName, alias), attribute);

            IMilestone milestone = new Milestone(1, 1);
            milestone.CountSmallerThan.Add(new TableAttribute(tableName, attribute), 0);

            // ACT
            var result = milestone.IsAnySmallerThanNoAlias(attrOrg);

            // ASSERT
            Assert.IsFalse(result);
        }

        #endregion

        #region IsAnyLargerThanNoAlias

        [TestMethod]
        [DataRow("A", "a", "V")]
        [DataRow("B", "someweirdAlias", "v")]
        public void Can_IsAnyLargerThanNoAlias_True(string tableName, string alias, string attribute)
        {
            // ARRANGE
            var attrOrg = new TableAttribute(new TableReferenceNode(tableName, alias), attribute);

            IMilestone milestone = new Milestone(1, 1);
            milestone.CountLargerThan.Add(new TableAttribute(tableName, attribute), 10UL);

            // ACT
            var result = milestone.IsAnyLargerThanNoAlias(attrOrg);

            // ASSERT
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("A", "a", "V")]
        [DataRow("B", "someweirdAlias", "v")]
        public void Can_IsAnyLargerThanNoAlias_False(string tableName, string alias, string attribute)
        {
            // ARRANGE
            var attrOrg = new TableAttribute(new TableReferenceNode(tableName, alias), attribute);

            IMilestone milestone = new Milestone(1, 1);
            milestone.CountLargerThan.Add(new TableAttribute(tableName, attribute), 0);

            // ACT
            var result = milestone.IsAnyLargerThanNoAlias(attrOrg);

            // ASSERT
            Assert.IsFalse(result);
        }

        #endregion

        #region GetTotalAbstractStorageUse

        [TestMethod]
        public void Can_GetTotalAbstractStorageUse_SingleType()
        {
            // ARRANGE
            int lowestValue = 50;
            long count = 50;

            var value1Attr = new TableAttribute("A", "v");
            ulong value1 = 10;
            var value2Attr = new TableAttribute("B", "v");
            ulong value2 = 50;
            var value3Attr = new TableAttribute("C", "v");
            ulong value3 = 1000;

            IMilestone milestone = new Milestone(lowestValue, count);
            milestone.CountLargerThan.Add(value1Attr, value1);
            milestone.CountLargerThan.Add(value2Attr, value2);
            milestone.CountLargerThan.Add(value3Attr, value3);

            // ACT
            var result = milestone.GetTotalAbstractStorageUse();

            // ASSERT
            // Size(lowestValue) + Size(count) + Size(value1) + Size(value2) + Size(value3)
            Assert.AreEqual(32UL + 64UL + 64UL + 64UL + 64UL, result);
        }

        [TestMethod]
        public void Can_GetTotalAbstractStorageUse_Mixed()
        {
            // ARRANGE
            int lowestValue = 50;
            long count = 50;

            var value1Attr = new TableAttribute("A", "v");
            ulong value1 = 10;
            var value2Attr = new TableAttribute("B", "v");
            int value2 = 50;
            var value3Attr = new TableAttribute("C", "v");
            byte value3 = 1;

            IMilestone milestone = new Milestone(lowestValue, count);
            milestone.CountLargerThan.Add(value1Attr, value1);
            milestone.CountLargerThan.Add(value2Attr, value2);
            milestone.CountLargerThan.Add(value3Attr, value3);

            // ACT
            var result = milestone.GetTotalAbstractStorageUse();

            // ASSERT
            // Size(lowestValue) + Size(count) + Size(value1) + Size(value2) + Size(value3)
            Assert.AreEqual(32UL + 64UL + 64UL + 32UL + 8UL, result);
        }

        #endregion
    }
}
