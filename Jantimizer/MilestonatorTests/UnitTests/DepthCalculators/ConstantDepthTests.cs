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
    public class ConstantDepthTests
    {
        [TestMethod]
        [DataRow(1, 5342, 23463)]
        [DataRow(100, 64, 264)]
        [DataRow(1000, 534, 6436)]
        [DataRow(10000, 765, 2347)]
        public void ConstantDepth_Base_Tests(int depth, long uniqueCount, long totalCount)
        {
            // ARRANGE
            IDepthCalculator depthCalculator = new ConstantDepth(depth);

            // ACT
            var calcedDepth = depthCalculator.GetDepth(uniqueCount, totalCount);

            // ASSERT
            Assert.AreEqual(depth, calcedDepth);
        }
    }
}
