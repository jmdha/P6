using Histograms;
using Histograms.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiserTest;
using QueryParser.Models;
using System;
using QueryOptimiser.Cost.Nodes;
using Histograms.Models;
using QueryOptimiser.Cost.Calculations;

namespace QueryOptimiserTest.Cost.Calculators
{
    [TestClass]
    public class DeviationEstimateTest
    {
        #region GetCertainty
        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(1, 2, 0.5)]
        [DataRow(1, 10, 0.1)]
        [DataRow(100, 10, 10)]
        public void Can_GetCertainty(double stdDeviation, double range, double expResult)
        {
            // ARRANGE

            // ACT
            double bCertainty = DeviationEstimate.GetCertainty(stdDeviation, range);

            // ASSERTs
            Assert.AreEqual(expResult, bCertainty);
        }

        [TestMethod]
        public void Can_GetCertainty_If_RangeZero()
        {
            // ARRANGE

            // ACT
            double bCertainty = DeviationEstimate.GetCertainty(1, 0);

            // ASSERTs
            Assert.AreEqual(1, bCertainty);
        }

        [TestMethod]
        public void Can_GetCertainty_If_StandardDeviationZero()
        {
            // ARRANGE

            // ACT
            double bCertainty = DeviationEstimate.GetCertainty(0, 1);

            // ASSERTs
            Assert.AreEqual(1, bCertainty);
        }

        #endregion

        #region GetComparativeCertainty

        [TestMethod]
        [DataRow(1, 1, 1)]
        [DataRow(1, 2, 0.5)]
        [DataRow(2, 1, 0.5)]
        [DataRow(100, 1, 0.01)]
        public void Can_GetComparativeCertainty(double certainty, double comparisonCertainty, double expResult)
        {
            // ARRANGE

            // ACT
            double bCertainty = DeviationEstimate.GetComparativeCertainty(certainty, comparisonCertainty);

            // ASSERTs
            Assert.AreEqual(expResult, bCertainty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Cant_GetComparativeCertainty_IfComparisonIsZero()
        {
            // ARRANGE

            // ACT
            DeviationEstimate.GetComparativeCertainty(1, 0);
        }

        #endregion
    }
}