using Histograms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.HistogramModels
{
    [TestClass]
    public class HistogramEquiDepthTests
    {
        #region Constructor

        [TestMethod]
        public void Constructor_GeneratesBuckets()
        {
            // ARRANGE
            // ACT
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", "b", 1);

            // ASSERT
            Assert.IsNotNull(histogram.Buckets);
        }

        [TestMethod]
        [DataRow("A","b",1)]
        [DataRow("B D"," a b",10)]
        [DataRow("Q","aAAa1",50)]
        [DataRow("Name","q",20000)]
        public void Constructor_SetsProperties(string tableName, string attributeName, int depth)
        {
            // ARRANGE
            // ACT
            HistogramEquiDepth histogram = new HistogramEquiDepth(tableName, attributeName, depth);

            // ASSERT
            Assert.AreEqual(tableName, histogram.TableName);
            Assert.AreEqual(attributeName, histogram.AttributeName);
            Assert.AreEqual(depth, histogram.Depth);
        }

        #endregion

        #region GenerateHistogram Table

        [TestMethod]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 3, 2)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 2, 3)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 10, 1)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 1, 5)]
        public void Can_GenerateHistogram_Table(string columnName, int[] rows, int depth, int expBucketCount)
        {
            // ARRANGE
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", columnName, depth);
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn(columnName, typeof(int)));
            foreach(int row in rows)
                table.Rows.Add(AddRow(table, new int[] { row }));

            // ACT
            histogram.GenerateHistogram(table, columnName);

            // ASSERT
            Assert.AreEqual(expBucketCount, histogram.Buckets.Count);
        }

        #endregion

        #region GenerateHistogram List

        [TestMethod]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 3, 2)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 2, 3)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 10, 1)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 1, 5)]
        public void Can_GenerateHistogram_List(string columnName, int[] rows, int depth, int expBucketCount)
        {
            // ARRANGE
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", columnName, depth);

            // ACT
            histogram.GenerateHistogram(rows.Cast<IComparable>().ToList());

            // ASSERT
            Assert.AreEqual(expBucketCount, histogram.Buckets.Count);
        }

        #endregion

        #region Private Test Methods

        private DataRow AddRow(DataTable dt, int[] data)
        {
            DataRow row = dt.NewRow();
            for (int i = 0; i < data.Length; i++)
                row[i] = data[i];
            return row;
        }

        #endregion
    }
}
