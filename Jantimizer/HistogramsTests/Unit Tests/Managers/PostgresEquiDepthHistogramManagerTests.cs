using DatabaseConnector;
using DatabaseConnector.Connectors;
using Histograms;
using Histograms.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.Managers
{
    [TestClass]
    public class PostgresEquiDepthHistogramManagerTests
    {
        #region Constructor

        [TestMethod]
        public void Constructor_SetsConnector()
        {
            // ARRANGE
            // ACT
            IHistogramManager<IHistogram, IDbConnector> manager = new PostgresEquiDepthHistogramManager("", 10);

            // ASSERT
            Assert.IsNotNull(manager.DbConnector);
            Assert.IsInstanceOfType(manager.DbConnector, typeof(PostgreSqlConnector));
        }

        [TestMethod]
        public void Constructor_HistogramList()
        {
            // ARRANGE
            // ACT
            IHistogramManager<IHistogram, IDbConnector> manager = new PostgresEquiDepthHistogramManager("", 10);

            // ASSERT
            Assert.IsNotNull(manager.Histograms);
            Assert.IsInstanceOfType(manager.Histograms, typeof(List<IHistogram>));
        }

        [TestMethod]
        [DataRow(5)]
        public void Constructor_SetsProperties(int depth)
        {
            // ARRANGE
            // ACT
            PostgresEquiDepthHistogramManager manager = new PostgresEquiDepthHistogramManager("", depth);

            // ASSERT
            Assert.AreEqual(depth, manager.Depth);
        }

        #endregion

        #region AddHistogram IHistogram

        [TestMethod]
        public void Can_AddHistogram_IHistogram()
        {
            // ARRANGE
            // ACT
            IHistogramManager<IHistogram, IDbConnector> manager = new PostgresEquiDepthHistogramManager("", 10);

            // ASSERT
            Assert.IsNotNull(manager.DbConnector);
            Assert.IsInstanceOfType(manager.DbConnector, typeof(PostgreSqlConnector));
        }

        #endregion
    }
}
