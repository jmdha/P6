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

namespace HistogramsTests.Unit_Tests.Managers
{
    [TestClass]
    public class MySQLEquiDepthHistogramManagerTests
    {
        #region Constructor

        [TestMethod]
        public void Constructor_SetsConnector()
        {
            // ARRANGE
            // ACT
            IHistogramManager<IHistogram, IDbConnector> manager = new MySQLEquiDepthHistogramManager(new ConnectionProperties(), 10);

            // ASSERT
            Assert.IsNotNull(manager.DbConnector);
            Assert.IsInstanceOfType(manager.DbConnector, typeof(DatabaseConnector.Connectors.MySqlConnector));
        }

        [TestMethod]
        public void Constructor_HistogramList()
        {
            // ARRANGE
            // ACT
            IHistogramManager<IHistogram, IDbConnector> manager = new MySQLEquiDepthHistogramManager(new ConnectionProperties(), 10);

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
            MySQLEquiDepthHistogramManager manager = new MySQLEquiDepthHistogramManager(new ConnectionProperties(), depth);

            // ASSERT
            Assert.AreEqual(depth, manager.Depth);
        }

        #endregion

        #region Properties

        [TestMethod]
        [DataRow(new string[] { "A", "B", "C" }, "a", new string[] { "A", "B", "C" })]
        [DataRow(new string[] { "A", "B   ", "C" }, "a", new string[] { "A", "B   ", "C" })]
        [DataRow(new string[] { " b", "Abbaaab", "C" }, "a", new string[] { " b", "Abbaaab", "C" })]
        public void Can_Get_Tables(string[] tableNames, string attribute, string[] expectedResult)
        {
            // ARRANGE
            IHistogramManager<IHistogram, IDbConnector> manager = new MySQLEquiDepthHistogramManager(new ConnectionProperties(), 10);
            foreach (string tableName in tableNames)
                manager.AddHistogram(new HistogramEquiDepth(tableName, attribute, 10));

            // ACT
            var result = manager.Tables;

            // ASSERT
            Assert.AreEqual(expectedResult[0], result[0]);
            Assert.AreEqual(expectedResult[1], result[1]);
            Assert.AreEqual(expectedResult[2], result[2]);
        }

        [TestMethod]
        [DataRow(new string[] { "a", "b", "c" }, "A", new string[] { "A.a", "A.b", "A.c" })]
        [DataRow(new string[] { "b", " c", "a" }, "B", new string[] { "B.b", "B. c", "B.a" })]
        [DataRow(new string[] { "c", "b", "t" }, "C", new string[] { "C.c", "C.b", "C.t" })]
        public void Can_Get_Attributes(string[] attributeName, string tableName, string[] expectedResult)
        {
            // ARRANGE
            IHistogramManager<IHistogram, IDbConnector> manager = new MySQLEquiDepthHistogramManager(new ConnectionProperties(), 10);
            foreach (string attribute in attributeName)
                manager.AddHistogram(new HistogramEquiDepth(tableName, attribute, 10));

            // ACT
            var result = manager.Attributes;

            // ASSERT
            Assert.AreEqual(expectedResult[0], result[0]);
            Assert.AreEqual(expectedResult[1], result[1]);
            Assert.AreEqual(expectedResult[2], result[2]);
        }

        #endregion

        #region AddHistogram

        [TestMethod]
        [DataRow("A", "v")]
        [DataRow(" B", "s")]
        [DataRow("C ", "  v")]
        public void Can_AddHistogram(string tableName, string attributeName)
        {
            // ARRANGE
            IHistogramManager<IHistogram, IDbConnector> manager = new MySQLEquiDepthHistogramManager(new ConnectionProperties(), 10);
            IHistogram histogram = new HistogramEquiDepth(tableName, attributeName, 10);

            // ACT
            manager.AddHistogram(histogram);

            // ASSERT
            Assert.AreEqual(histogram, manager.Histograms[0]);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow("      ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Cant_AddHistogram_IfTableNameEmpty(string tableName)
        {
            // ARRANGE
            IHistogramManager<IHistogram, IDbConnector> manager = new MySQLEquiDepthHistogramManager(new ConnectionProperties(), 10);
            IHistogram histogram = new HistogramEquiDepth(tableName, "a", 10);

            // ACT
            manager.AddHistogram(histogram);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow("      ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Cant_AddHistogram_IfAttributeNameEmpty(string attributeName)
        {
            // ARRANGE
            IHistogramManager<IHistogram, IDbConnector> manager = new MySQLEquiDepthHistogramManager(new ConnectionProperties(), 10);
            IHistogram histogram = new HistogramEquiDepth("A", attributeName, 10);

            // ACT
            manager.AddHistogram(histogram);
        }

        #endregion
    }
}
