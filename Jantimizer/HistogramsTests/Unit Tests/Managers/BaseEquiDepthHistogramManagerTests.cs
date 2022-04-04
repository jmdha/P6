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
            // ACT
            EquiDepthHistogramManager manager = new EquiDepthHistogramManager(new DataGathererStub(), depth);

            // ASSERT
            Assert.AreEqual(depth, manager.Depth);
        }

        #endregion

    }
}
