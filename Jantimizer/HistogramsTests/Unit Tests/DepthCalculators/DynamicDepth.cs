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
    public class DynaDepth
    {
        [TestMethod]
        [DataRow(5342, 23463)]
        [DataRow(64, 264)]
        [DataRow(534, 6436)]
        [DataRow(70165, 2301047)]
        [DataRow(10000000, 23410017)]
        public void DynamicDepth_Base_Tests(long uniqueCount, long totalCount)
        {
            // ARRANGE
            DepthCalculator getDepth = new DynamicDepth().GetDepth;

            // ACT
            var calcedDepth = getDepth(uniqueCount, totalCount);

            // ASSERT
            Assert.IsTrue(0 < calcedDepth);
            Assert.IsFalse(calcedDepth > uniqueCount);
        }
    }
}
