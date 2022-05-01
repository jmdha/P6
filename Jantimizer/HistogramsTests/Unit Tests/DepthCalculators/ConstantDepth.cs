using Histograms;
using Histograms.DepthCalculators;
using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.DepthCalculators
{
    [TestClass]
    public class ConstDepth
    {
        [TestMethod]
        [DataRow(1, 5342, 23463)]
        [DataRow(100, 64, 264)]
        [DataRow(1000, 534, 6436)]
        [DataRow(10000, 765, 2347)]
        public void ConstantDepth_Base_Tests(int depth, long uniqueCount, long totalCount)
        {
            // ARRANGE
            DepthCalculator getDepth = new ConstantDepth(depth).GetDepth;

            // ACT
            var calcedDepth = getDepth(uniqueCount, totalCount);

            // ASSERT
            Assert.AreEqual(depth, calcedDepth);
        }
    }
}
