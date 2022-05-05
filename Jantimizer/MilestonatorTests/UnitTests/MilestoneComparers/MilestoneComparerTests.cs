using Microsoft.VisualStudio.TestTools.UnitTesting;
using Milestoner.MilestoneComparers;
using Milestoner.Models;
using Milestoner.Models.Milestones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace MilestonatorTests.UnitTests.MilestoneComparers
{
    [TestClass]
    public class MilestoneComparerTests
    {
        [TestMethod]
        public void Can_DoMilestoneComparisons_Simple()
        {
            // ARRANGE
            TableAttribute attr1 = new TableAttribute("A", "v");
            TableAttribute attr2 = new TableAttribute("B", "v");
            TableAttribute attr3 = new TableAttribute("C", "v");

            var expLargerThanAttr1Count = new int[] { 3, 3, 2 };
            var expLessThanAttr1Count = new int[] { 0, 1, 2 };
            var expLargerThanAttr2Count = new int[] { 3, 2, 1 };
            var expLessThanAttr2Count = new int[] { 1, 2, 2 };
            var expLargerThanAttr3Count = new int[] { 1, 1, 0 };
            var expLessThanAttr3Count = new int[] { 2, 3, 3 };

            Dictionary<TableAttribute, List<IMilestone>> milestones = new Dictionary<TableAttribute, List<IMilestone>>() {
                { attr1, new List<IMilestone>() { new Milestone(0, 10), new Milestone(10, 10), new Milestone(20, 10) } },
                { attr2, new List<IMilestone>() { new Milestone(10, 10), new Milestone(20, 10), new Milestone(30, 10) } },
                { attr3, new List<IMilestone>() { new Milestone(40, 10), new Milestone(50, 10), new Milestone(60, 10) } }
            };
            Dictionary<TableAttribute, List<ValueCount>> data = new Dictionary<TableAttribute, List<ValueCount>>() {
                { attr1, new List<ValueCount>() { new ValueCount(0, 10), new ValueCount(10, 10), new ValueCount(20, 10) } },
                { attr2, new List<ValueCount>() { new ValueCount(10, 10), new ValueCount(20, 10), new ValueCount(30, 10) } },
                { attr3, new List<ValueCount>() { new ValueCount(40, 10), new ValueCount(50, 10), new ValueCount(60, 10) } }
            };

            IMilestoneComparers comparer = new MilestoneComparer(milestones, data);

            // ACT
            comparer.DoMilestoneComparisons();

            // ASSERT
            for (int i = 0; i < comparer.Milestones[attr1].Count; i++)
            {
                Assert.AreEqual(expLargerThanAttr1Count[i], comparer.Milestones[attr1][i].CountLargerThan.Count);
                Assert.AreEqual(expLessThanAttr1Count[i], comparer.Milestones[attr1][i].CountSmallerThan.Count);
            }
            for (int i = 0; i < comparer.Milestones[attr2].Count; i++)
            {
                Assert.AreEqual(expLargerThanAttr2Count[i], comparer.Milestones[attr2][i].CountLargerThan.Count);
                Assert.AreEqual(expLessThanAttr2Count[i], comparer.Milestones[attr2][i].CountSmallerThan.Count);
            }
            for (int i = 0; i < comparer.Milestones[attr3].Count; i++)
            {
                Assert.AreEqual(expLargerThanAttr3Count[i], comparer.Milestones[attr3][i].CountLargerThan.Count);
                Assert.AreEqual(expLessThanAttr3Count[i], comparer.Milestones[attr3][i].CountSmallerThan.Count);
            }
        }

        [TestMethod]
        public void Can_DoMilestoneComparisons_Complex_1()
        {
            // ARRANGE
            TableAttribute attr1 = new TableAttribute("A", "v");
            TableAttribute attr2 = new TableAttribute("B", "v");
            TableAttribute attr3 = new TableAttribute("C", "v");

            // Key 1, CountLargerThan for each of the attributes
            var expLargerThanAttr1 = new ulong[] { 10, 20, 30 };
            // Key 1, CountLessThan for each of the attributes
            var expLessThanAttr1 = new ulong[] { 10, 0, 0 };
            // Key 2, CountLargerThan for each of the attributes
            var expLargerThanAttr2 = new ulong[] { 0, 10, 30 };
            // Key 2, CountLessThan for each of the attributes
            var expLessThanAttr2 = new ulong[] { 20, 10, 0 };
            // Key 3, CountLargerThan for each of the attributes
            var expLargerThanAttr3 = new ulong[] { 0, 0, 30 };
            // Key 3, CountLessThan for each of the attributes
            var expLessThanAttr3 = new ulong[] { 30, 20, 0 };

            Dictionary<TableAttribute, List<IMilestone>> milestones = new Dictionary<TableAttribute, List<IMilestone>>() {
                { attr1, new List<IMilestone>() { new Milestone(0, 10), new Milestone(10, 10), new Milestone(20, 10) } },
                                               // Key 1                  Key 2                  Key 3
                { attr2, new List<IMilestone>() { new Milestone(10, 10), new Milestone(20, 10), new Milestone(30, 10) } },
                { attr3, new List<IMilestone>() { new Milestone(40, 10), new Milestone(50, 10), new Milestone(60, 10) } }
            };
            Dictionary<TableAttribute, List<ValueCount>> data = new Dictionary<TableAttribute, List<ValueCount>>() {
                { attr1, new List<ValueCount>() { new ValueCount(0, 10), new ValueCount(10, 10), new ValueCount(20, 10) } },
                { attr2, new List<ValueCount>() { new ValueCount(10, 10), new ValueCount(20, 10), new ValueCount(30, 10) } },
                { attr3, new List<ValueCount>() { new ValueCount(40, 10), new ValueCount(50, 10), new ValueCount(60, 10) } }
            };

            IMilestoneComparers comparer = new MilestoneComparer(milestones, data);

            // ACT
            comparer.DoMilestoneComparisons();

            // ASSERT
            for (int i = 0; i < comparer.Milestones[attr2].Count; i++)
            {
                int keyCounter = 0;
                foreach(var key in comparer.Milestones.Keys)
                {
                    if (i == 0)
                    {
                        // Key 1
                        Assert.AreEqual(expLargerThanAttr1[keyCounter], Convert.ToUInt64(comparer.Milestones[attr2][i].GetCountLargerThanNoAlias(key)));
                        Assert.AreEqual(expLessThanAttr1[keyCounter], Convert.ToUInt64(comparer.Milestones[attr2][i].GetCountSmallerThanNoAlias(key)));
                    }
                    else if (i == 1)
                    {
                        // Key 2
                        Assert.AreEqual(expLargerThanAttr2[keyCounter], Convert.ToUInt64(comparer.Milestones[attr2][i].GetCountLargerThanNoAlias(key)));
                        Assert.AreEqual(expLessThanAttr2[keyCounter], Convert.ToUInt64(comparer.Milestones[attr2][i].GetCountSmallerThanNoAlias(key)));
                    }
                    else if (i == 2)
                    {
                        // Key 3
                        Assert.AreEqual(expLargerThanAttr3[keyCounter], Convert.ToUInt64(comparer.Milestones[attr2][i].GetCountLargerThanNoAlias(key)));
                        Assert.AreEqual(expLessThanAttr3[keyCounter], Convert.ToUInt64(comparer.Milestones[attr2][i].GetCountSmallerThanNoAlias(key)));
                    }
                    keyCounter++;
                }
            }
        }

        [TestMethod]
        public void Can_DoMilestoneComparisons_WontCompareMixedTypes()
        {
            // ARRANGE
            TableAttribute attr1 = new TableAttribute("A", "v");
            TableAttribute attr2 = new TableAttribute("B", "v");
            TableAttribute attr3 = new TableAttribute("C", "v");

            Dictionary<TableAttribute, List<IMilestone>> milestones = new Dictionary<TableAttribute, List<IMilestone>>() {
                { attr1, new List<IMilestone>() { new Milestone(0, 10), new Milestone(10, 10), new Milestone(20, 10) } },
                { attr2, new List<IMilestone>() { new Milestone("a", 10), new Milestone("b", 10), new Milestone("c", 10) } },
                { attr3, new List<IMilestone>() { new Milestone(40, 10), new Milestone(50, 10), new Milestone(60, 10) } }
            };
            Dictionary<TableAttribute, List<ValueCount>> data = new Dictionary<TableAttribute, List<ValueCount>>() {
                { attr1, new List<ValueCount>() { new ValueCount(0, 10), new ValueCount(10, 10), new ValueCount(20, 10) } },
                { attr2, new List<ValueCount>() { new ValueCount("a", 10), new ValueCount("b", 10), new ValueCount("c", 10) } },
                { attr3, new List<ValueCount>() { new ValueCount(40, 10), new ValueCount(50, 10), new ValueCount(60, 10) } }
            };

            IMilestoneComparers comparer = new MilestoneComparer(milestones, data);

            // ACT
            comparer.DoMilestoneComparisons();

            // ASSERT
            Assert.IsTrue(comparer.Milestones[attr2][0].CountLargerThan.ContainsKey(attr2));
            Assert.IsFalse(comparer.Milestones[attr2][0].CountLargerThan.ContainsKey(attr1));
            Assert.IsFalse(comparer.Milestones[attr2][0].CountLargerThan.ContainsKey(attr3));

            Assert.AreEqual(0, comparer.Milestones[attr2][0].CountSmallerThan.Count);
            Assert.AreEqual(1, comparer.Milestones[attr2][1].CountSmallerThan.Count);
            Assert.AreEqual(1, comparer.Milestones[attr2][2].CountSmallerThan.Count);

            Assert.AreEqual(1, comparer.Milestones[attr2][0].CountLargerThan.Count);
            Assert.AreEqual(1, comparer.Milestones[attr2][1].CountLargerThan.Count);
            Assert.AreEqual(0, comparer.Milestones[attr2][2].CountLargerThan.Count);
        }
    }
}
