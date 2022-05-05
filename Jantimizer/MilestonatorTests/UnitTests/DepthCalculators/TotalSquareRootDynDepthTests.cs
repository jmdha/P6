using Microsoft.VisualStudio.TestTools.UnitTesting;
using Milestoner.DepthCalculators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilestonatorTests.UnitTests.DepthCalculators
{
    [TestClass]
    public class TotalSquareRootDynDepthTests
    {
        [TestMethod]
        [DataRow(5342, 23463, -50, 10, 25)]
        [DataRow(64, 264, -50, 10, 25)]
        [DataRow(534, 6436, -50, 10, 25)]
        [DataRow(70165, 2301047, -50, 10, 25)]
        [DataRow(10000000, 23410017, -50, 10, 25)]
        public void UniqueDynamicDepth_Base_Tests(long uniqueCount, long totalCount, double yOffset, double rootMultiplier, double rootOffset)
        {
            // ARRANGE
            IDepthCalculator depthCalculator = new TotalSquareRootDynDepth(yOffset, rootMultiplier, rootOffset);

            // ACT
            var calcedDepth = depthCalculator.GetDepth(uniqueCount, totalCount);

            // ASSERT
            Assert.IsTrue(0 < calcedDepth);
            Assert.IsFalse(calcedDepth > uniqueCount);
        }
    }
}
