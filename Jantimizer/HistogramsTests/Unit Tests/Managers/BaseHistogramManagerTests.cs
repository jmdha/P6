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
using Histograms.DataGatherers;
using HistogramsTests.Stubs;
using Histograms.DepthCalculators;

namespace HistogramsTests.Unit_Tests.Managers
{
    [TestClass]
    public class BaseHistogramManagerTests
    {
        #region Properties

        [TestMethod]
        [DataRow(new string[] { "A", "B", "C" }, "a", new string[] { "A", "B", "C" })]
        [DataRow(new string[] { "A", "B   ", "C" }, "a", new string[] { "A", "B   ", "C" })]
        [DataRow(new string[] { " b", "Abbaaab", "C" }, "a", new string[] { " b", "Abbaaab", "C" })]
        public void Can_Get_Tables(string[] tableNames, string attribute, string[] expectedResult)
        {
            // ARRANGE
            IHistogramManager manager = new BaseHistogramManagerStub();
            foreach (string tableName in tableNames)
                manager.AddHistogram(new HistogramEquiDepth(tableName, attribute, new ConstantDepth(10).GetDepth));

            // ACT
            var result = manager.Tables;

            // ASSERT
            Assert.AreEqual(expectedResult[0].ToLower(), result[0].ToLower());
            Assert.AreEqual(expectedResult[1].ToLower(), result[1].ToLower());
            Assert.AreEqual(expectedResult[2].ToLower(), result[2].ToLower());
        }

        [TestMethod]
        [DataRow(new string[] { "a", "b", "c" }, "A", new string[] { "A.a", "A.b", "A.c" })]
        [DataRow(new string[] { "b", " c", "a" }, "B", new string[] { "B.b", "B. c", "B.a" })]
        [DataRow(new string[] { "c", "b", "t" }, "C", new string[] { "C.c", "C.b", "C.t" })]
        public void Can_Get_Attributes(string[] attributeName, string tableName, string[] expectedResult)
        {
            // ARRANGE
            IHistogramManager manager = new BaseHistogramManagerStub();
            foreach (string attribute in attributeName)
                manager.AddHistogram(new HistogramEquiDepth(tableName, attribute, new ConstantDepth(10).GetDepth));

            // ACT
            var result = manager.Attributes;

            // ASSERT
            Assert.AreEqual(expectedResult[0].ToLower(), result[0].ToLower());
            Assert.AreEqual(expectedResult[1].ToLower(), result[1].ToLower());
            Assert.AreEqual(expectedResult[2].ToLower(), result[2].ToLower());
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
            IHistogramManager manager = new BaseHistogramManagerStub();
            IHistogram histogram = new HistogramEquiDepthVariance(tableName, attributeName, new ConstantDepth(10).GetDepth);

            // ACT
            manager.AddHistogram(histogram);

            // ASSERT
            Assert.AreEqual(histogram, manager.GetHistogram(tableName, attributeName));
        }

        [TestMethod]
        [DataRow("A", "v")]
        [DataRow(" B", "s")]
        [DataRow("C ", "  v")]
        [ExpectedException(typeof(ArgumentException))]
        public void Cant_duplicate_AddHistogram(string tableName, string attributeName)
        {
            // ARRANGE
            IHistogramManager manager = new BaseHistogramManagerStub();
            IHistogram histogram = new HistogramEquiDepthVariance(tableName, attributeName, new ConstantDepth(10).GetDepth);

            // ACT
            manager.AddHistogram(histogram);
            manager.AddHistogram(histogram);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow("      ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Cant_AddHistogram_IfTableNameEmpty(string tableName)
        {
            // ARRANGE
            IHistogramManager manager = new BaseHistogramManagerStub();
            IHistogram histogram = new HistogramEquiDepthVariance(tableName, "a", new ConstantDepth(10).GetDepth);

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
            IHistogramManager manager = new BaseHistogramManagerStub();
            IHistogram histogram = new HistogramEquiDepth("A", attributeName, new ConstantDepth(10).GetDepth);

            // ACT
            manager.AddHistogram(histogram);
        }

        #endregion

        #region ClearHistogram
        [TestMethod]
        public void ClearHistogram()
        {
            // ARRANGE
            IHistogramManager manager = new BaseHistogramManagerStub();
            var expectedHistogram = new HistogramEquiDepth("t", "a", new ConstantDepth(10).GetDepth);
            manager.AddHistogram(expectedHistogram);

            Assert.IsNotNull(manager.GetHistogram("t", "a"));

            // ACT
            manager.ClearHistograms();

            // ASSERT
            Assert.ThrowsException<KeyNotFoundException>(() => manager.GetHistogram("t", "a"));
        }
        #endregion

        #region GetHistogram
        [TestMethod]
        [DataRow("A", "B")]
        [DataRow("B", "A")]
        public void GetHistogram_Single(string table, string attribute)
        {
            // ARRANGE
            IHistogramManager manager = new BaseHistogramManagerStub();
            var expectedHistogram = new HistogramEquiDepthVariance(table, attribute, new ConstantDepth(10).GetDepth);
            manager.AddHistogram(expectedHistogram);

            // ACT
            IHistogram actualHistogram = manager.GetHistogram(table, attribute);

            // ASSERT
            Assert.AreEqual(expectedHistogram, actualHistogram);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        [DataRow("A", "b")]
        [DataRow("B", "a")]
        [DataRow("B", "b")]
        public void GetHistogram_NoHit(string table, string attribute)
        {
            // ARRANGE
            IHistogramManager manager = new BaseHistogramManagerStub();
            manager.AddHistogram(
                new HistogramEquiDepthVariance("A", "a", new ConstantDepth(10).GetDepth)
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
            IHistogramManager manager = new BaseHistogramManagerStub();
            for (int i = 0; i < tables.Length; i++)
                manager.AddHistogram(new HistogramEquiDepth(tables[i], attributes[i], new ConstantDepth(1).GetDepth));

            // ACT
            IHistogram actualHistogram = manager.GetHistogram(table, attribute);

            // ASSERT
            Assert.AreEqual(table, actualHistogram.TableName);
            Assert.AreEqual(attribute, actualHistogram.AttributeName);
        }
        #endregion

    }
}
