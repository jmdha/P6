using Histograms.Models;
using HistogramsTests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.Models.Histograms
{
    [TestClass]
    public class HistogramEquiDepthVarianceTests
    {
        #region Constructor

        [TestMethod]
        public void Constructor_GeneratesBuckets()
        {
            // ARRANGE
            // ACT
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", 1);

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
            IDepthHistogram histogram = new HistogramEquiDepthVariance(tableName, attributeName, depth);

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
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", columnName, depth);
            DataTable table = DataTableHelper.GetDatatable(columnName, typeof(int));
            foreach(int row in rows)
                DataTableHelper.AddRow(table, new int[] { row });

            // ACT
            histogram.GenerateHistogram(table, columnName);

            // ASSERT
            Assert.AreEqual(expBucketCount, histogram.Buckets.Count);
        }

        [TestMethod]
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 14)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 1)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 48)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 11998)]
        public void Can_GenerateHistogram_Table_Variance(int[] rows, int expVariance)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", 5);
            DataTable table = DataTableHelper.GetDatatable("b", typeof(int));
            foreach (int row in rows)
                DataTableHelper.AddRow(table, new int[] { row });

            // ACT
            histogram.GenerateHistogram(table, "b");

            // ASSERT
            if (histogram.Buckets[0] is IHistogramBucketVariance var)
                Assert.AreEqual(expVariance, var.Variance);
            else
                Assert.Fail();
        }

        [TestMethod]
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 20)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 3)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 40)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 6002)]
        public void Can_GenerateHistogram_Table_Mean(int[] rows, int expMean)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", 5);
            DataTable table = DataTableHelper.GetDatatable("b", typeof(int));
            foreach (int row in rows)
                DataTableHelper.AddRow(table, new int[] { row });

            // ACT
            histogram.GenerateHistogram(table, "b");

            // ASSERT
            if (histogram.Buckets[0] is IHistogramBucketVariance var)
                Assert.AreEqual(expMean, var.Mean);
            else
                Assert.Fail();
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
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", columnName, depth);

            // ACT
            histogram.GenerateHistogram(rows.Cast<IComparable>().ToList());

            // ASSERT
            Assert.AreEqual(expBucketCount, histogram.Buckets.Count);
        }

        [TestMethod]
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 14)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 1)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 48)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 11998)]
        public void Can_GenerateHistogram_List_Variance(int[] rows, int expVariance)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", 5);

            // ACT
            histogram.GenerateHistogram(rows.Cast<IComparable>().ToList());

            // ASSERT
            if (histogram.Buckets[0] is IHistogramBucketVariance var)
                Assert.AreEqual(expVariance, var.Variance);
            else
                Assert.Fail();
        }

        [TestMethod]
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 20)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 3)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 40)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 6002)]
        public void Can_GenerateHistogram_List_Mean(int[] rows, int expMean)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", 5);

            // ACT
            histogram.GenerateHistogram(rows.Cast<IComparable>().ToList());

            // ASSERT
            if (histogram.Buckets[0] is IHistogramBucketVariance var)
                Assert.AreEqual(expMean, var.Mean);
            else
                Assert.Fail();
        }

        #endregion
    }
}
