using DatabaseConnector;
using DatabaseConnector.Connectors;
using Histograms;
using Histograms.Models;
using Histograms.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;
using HistogramsTests.Stubs;
using Histograms.DepthCalculators;

namespace HistogramsTests.Unit_Tests.Managers
{
    [TestClass]
    public class BaseEquiDepthHistogramManagerTests
    {
        #region Constructor

        [TestMethod]
        [DataRow(5)]
        public void Constructor_SetsProperties(int depth)
        {
            // ARRANGE
            DepthCalculator constDepthDelegate = new ConstantDepth(depth).GetDepth;
            // ACT
            EquiDepthHistogramManager manager = new EquiDepthHistogramManager(new DataGathererStub(), constDepthDelegate);

            // ASSERT
            Assert.AreEqual(depth, manager.GetDepth(5, 123));
            Assert.AreEqual(depth, manager.GetDepth(643, 133));
        }

        #endregion

    }
}
