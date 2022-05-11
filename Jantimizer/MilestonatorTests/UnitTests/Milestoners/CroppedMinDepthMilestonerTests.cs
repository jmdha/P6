using Microsoft.VisualStudio.TestTools.UnitTesting;
using MilestonatorTests.Stubs;
using Milestoner;
using Milestoner.DepthCalculators;
using Milestoner.Milestoners;
using Milestoner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace MilestonatorTests.UnitTests.Milestoners
{
    [TestClass]
    public class CroppedMinDepthMilestonerTests
    {
        #region GenerateHistogram Table

        [TestMethod]
        //                    Row IDs: 1  2  3  4  5
        [DataRow("a", "v", new int[] { 1, 2, 3, 4, 5 }, 2, 5)]
        [DataRow("b", "v", new int[] { 9, 0, 9, 0, 9 }, 2, 3)]
        [DataRow("c", "v", new int[] { 0, 0, 9, 0, 0 }, 2, 1)]
        [DataRow("d", "v", new int[] { 5, 5, 5, 5, 5 }, 10, 3)]
        [DataRow("e", "v", new int[] { 5, 4, 3, 2, 1 }, 100, 1)]
        [DataRow("f", "v", new int[] { 9, 0, 0, 1, 1 }, 1, 3)]
        [DataRow("g", "v", new int[] { 9, 1, 2, 1, 5 }, 3, 4)]
        public void Can_AddMilestonesFromValueCount_Table(string tableName, string attrName, int[] counts, int depth, int expMilestoneCount)
        {
            // ARRANGE
            var attr = new TableAttribute(tableName, attrName);
            IMilestoner milestoner = new CroppedMinDepthMilestoner(new TestDataGathere(), new ConstantDepth(depth));
            var valueList = new List<ValueCount>();
            for (int i = 0; i < counts.Length; i++)
                if (counts[i] > 0)
                    valueList.Add(new ValueCount(i, counts[i]));

            // ACT
            milestoner.AddMilestonesFromValueCount(attr, valueList);

            // ASSERT
            Assert.AreEqual(expMilestoneCount, milestoner.Milestones[attr].Count);
        }

        #endregion
    }
}
