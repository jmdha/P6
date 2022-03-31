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
    public class PostgresEquiDepthHistogramManagerTests
    {
        #region Constructor

        [TestMethod]
        public void Constructor_SetsConnector()
        {
            // ARRANGE
            // ACT
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);

            // ASSERT
            Assert.IsNotNull(manager.DbConnector);
            Assert.IsInstanceOfType(manager.DbConnector, typeof(PostgreSqlConnector));
        }

        [TestMethod]
        public void Constructor_HistogramList()
        {
            // ARRANGE
            // ACT
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);

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
            PostgresEquiDepthHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);

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
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
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
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
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
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
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
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
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
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            IHistogram histogram = new HistogramEquiDepth("A", attributeName, 10);

            // ACT
            manager.AddHistogram(histogram);
        }

        #endregion

        #region ClearHistogram
        [TestMethod]
        public void ClearHistogram()
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            manager.Histograms.Add(
                new HistogramEquiDepth("", "", 10)
            );

            if (manager.Histograms.Count == 0)
                throw new AssertFailedException("Histogram not added before clear");

            // ACT
            manager.ClearHistograms();

            // ASSERT
            Assert.AreEqual(0, manager.Histograms.Count);
        }
        #endregion

        #region GetHistogram
        [TestMethod]
        [DataRow("A", "B")]
        [DataRow("B", "A")]
        public void GetHistogram_Single(string table, string attribute)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            manager.Histograms.Add(
                new HistogramEquiDepth(table, attribute, 10)
            );

            // ACT
            IHistogram actualHistogram = manager.GetHistogram(table, attribute);

            // ASSERT
            Assert.AreEqual(manager.Histograms[0], actualHistogram);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DataRow("A", "b")]
        [DataRow("B", "a")]
        [DataRow("B", "b")]
        public void GetHistogram_NoHit(string table, string attribute)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            manager.Histograms.Add(
                new HistogramEquiDepth("A", "a", 10)
            );

            // ACT
            IHistogram actualHistogram = manager.GetHistogram(table, attribute);
        }

        [TestMethod]
        [DataRow("A", "a", new string[] { "A", "B", "C" }, new string[] { "a", "b", "c" })]
        [DataRow("B", "b", new string[] { "A", "B", "C" }, new string[] { "a", "b", "c" })]
        [DataRow("C", "c", new string[] { "A", "B", "C" }, new string[] { "a", "b", "c" })]
        public void GetHistogram_Multiple(string table, string attribute, string[] tables, string[] attributes)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            for (int i = 0; i < tables.Length; i++)
                manager.AddHistogram(new HistogramEquiDepth(tables[i], attributes[i], 1));

            // ACT
            IHistogram actualHistogram = manager.GetHistogram(table, attribute);

            // ASSERT
            Assert.AreEqual(table, actualHistogram.TableName);
            Assert.AreEqual(attribute, actualHistogram.AttributeName);
        }

        [TestMethod]
        [DataRow("A", "a", new string[] { "A", "A", "B", "B", "C", "C" }, new string[] { "a", "a", "b", "b", "c", "c" })]
        [DataRow("B", "b", new string[] { "A", "A", "B", "B", "C", "C" }, new string[] { "a", "a", "b", "b", "c", "c" })]
        [DataRow("C", "c", new string[] { "A", "A", "B", "B", "C", "C" }, new string[] { "a", "a", "b", "b", "c", "c" })]
        public void GetHistogram_MultipleDuplicate(string table, string attribute, string[] tables, string[] attributes)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            for (int i = 0; i < tables.Length; i++)
                manager.AddHistogram(new HistogramEquiDepth(tables[i], attributes[i], 1));

            // ACT
            IHistogram actualHistogram = manager.GetHistogram(table, attribute);

            // ASSERT
            Assert.AreEqual(table, actualHistogram.TableName);
            Assert.AreEqual(attribute, actualHistogram.AttributeName);
        }
        #endregion

        #region GetHistogramByTable
        [TestMethod]
        [DataRow("A")]
        [DataRow("B")]
        public void GetHistogramByTable_Single(string table)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            manager.Histograms.Add(
                new HistogramEquiDepth(table, "", 10)
            );

            // ACT
            List<IHistogram> actualHistograms = manager.GetHistogramsByTable(table);

            // ASSERT
            Assert.AreEqual(1, actualHistograms.Count);
            Assert.AreEqual(manager.Histograms[0], actualHistograms[0]);
        }

        [TestMethod]
        [DataRow("B", "a", new string[] { "A" })]
        [DataRow("D", "a", new string[] { "A, B, C" })]
        [DataRow("E", "a", new string[] { "A, A, B, C" })]
        public void GetHistogramByTable_SingleNoHit(string table, string attribute, string[] tables)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            for (int i = 0; i < tables.Length; i++)
                manager.AddHistogram(new HistogramEquiDepth(tables[i], attribute, 1));

            // ACT
            List<IHistogram> actualHistograms = manager.GetHistogramsByTable(table);

            // ASSERT
            Assert.AreEqual(0, actualHistograms.Count);
        }

        [TestMethod]
        [DataRow("A", 1, new string[] { "A", "B", "C" })]
        [DataRow("B", 1, new string[] { "A", "B", "C" })]
        [DataRow("C", 1, new string[] { "A", "B", "C" })]
        [DataRow("A", 2, new string[] { "A", "A", "B", "C" })]
        public void GetHistogramByTable_Multiple(string table, int hits, string[] tables)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            for (int i = 0; i < tables.Length; i++)
                manager.AddHistogram(new HistogramEquiDepth(tables[i], "b", 1));

            // ACT
            List<IHistogram> actualHistograms = manager.GetHistogramsByTable(table);

            // ASSERT
            Assert.AreEqual(hits, actualHistograms.Count);
        }
        #endregion

        #region GetHistogramByAttribute
        [TestMethod]
        [DataRow("a")]
        [DataRow("b")]
        public void GetHistogramByAttribute_Single(string attribute)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            manager.Histograms.Add(
                new HistogramEquiDepth("A", attribute, 10)
            );

            // ACT
            List<IHistogram> actualHistograms = manager.GetHistogramsByAttribute(attribute);

            // ASSERT
            Assert.AreEqual(1, actualHistograms.Count);
            Assert.AreEqual(manager.Histograms[0], actualHistograms[0]);
        }

        [TestMethod]
        [DataRow("A", "b", new string[] { "a" })]
        [DataRow("A", "d", new string[] { "a, b, c" })]
        [DataRow("A", "d", new string[] { "a, a, b, c" })]
        public void GetHistogramByAttribute_SingleNoHit(string table, string attribute, string[] attributes)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            for (int i = 0; i < attributes.Length; i++)
                manager.AddHistogram(new HistogramEquiDepth(table, attributes[i], 1));

            // ACT
            List<IHistogram> actualHistograms = manager.GetHistogramsByAttribute(attribute);

            // ASSERT
            Assert.AreEqual(0, actualHistograms.Count);
        }

        [TestMethod]
        [DataRow("a", 1, new string[] { "a", "b", "c" })]
        [DataRow("b", 1, new string[] { "a", "b", "c" })]
        [DataRow("c", 1, new string[] { "a", "b", "c" })]
        [DataRow("a", 2, new string[] { "a", "a", "b", "c" })]
        public void GetHistogramByAttribute_Multiple(string table, int hits, string[] attributes)
        {
            // ARRANGE
            IHistogramManager manager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
            for (int i = 0; i < attributes.Length; i++)
                manager.AddHistogram(new HistogramEquiDepth("A", attributes[i], 1));

            // ACT
            List<IHistogram> actualHistograms = manager.GetHistogramsByAttribute(table);

            // ASSERT
            Assert.AreEqual(hits, actualHistograms.Count);
        }
        #endregion
    }
}
