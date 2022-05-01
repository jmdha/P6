using Histograms;
using Histograms.DepthCalculators;
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
    public class HistogramMinDepthTests
    {
        #region Constructor

        [TestMethod]
        public void Constructor_GeneratesBuckets()
        {
            // ARRANGE
            // ACT
            IDepthHistogram histogram = new HistogramMinDepth("A", "b", new ConstantDepth(1).GetDepth);

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
            DepthCalculator getDepth = new ConstantDepth(depth).GetDepth;
            // ACT
            IDepthHistogram histogram = new HistogramMinDepth(tableName, attributeName, getDepth);

            // ASSERT
            Assert.AreEqual(tableName, histogram.TableName);
            Assert.AreEqual(attributeName, histogram.AttributeName);
            Assert.AreEqual(getDepth, histogram.GetDepth);
        }

        #endregion

        #region GenerateHistogram Table

        [TestMethod]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 3, 2)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 2, 3)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 10, 1)]
        [DataRow("b", new int[] { 1, 2, 3, 4, 5 }, 1, 5)]
        [DataRow("b", new int[] { 1, 1, 1, 2, 2 }, 2, 2)]
        [DataRow("b", new int[] { 0, 1, 1, 2, 2 }, 2, 2)]
        public void Can_GenerateHistogram_Table(string columnName, int[] rows, int depth, int expBucketCount)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramMinDepth("A", columnName, new ConstantDepth(depth).GetDepth);
            DataTable table = DataTableHelper.GetDatatable(columnName, typeof(int));
            foreach (int row in rows)
                DataTableHelper.AddRow(table, new int[] { row });

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
        [DataRow("b", new int[] { 1, 1, 1, 2, 2 }, 2, 2)]
        [DataRow("b", new int[] { 0, 1, 1, 2, 2 }, 2, 2)]
        public void Can_GenerateHistogram_List(string columnName, int[] rows, int depth, int expBucketCount)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramMinDepth("A", columnName, new ConstantDepth(depth).GetDepth);

            // ACT
            histogram.GenerateHistogram(rows.Cast<IComparable>().ToList());

            // ASSERT
            Assert.AreEqual(expBucketCount, histogram.Buckets.Count);
        }

        #endregion

        #region GenerateHistogramFromSorted

        [TestMethod]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 10, 1)]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 5, 1)]
        [DataRow(new int[] { 1, 2 }, new long[] { 20, 50 }, 10, 2)]
        [DataRow(new int[] { 1, 2 }, new long[] { 20, 50 }, 5, 2)]
        [DataRow(new int[] { 1, 2, 3 }, new long[] { 20, 50, 100 }, 10, 3)]
        [DataRow(new int[] { 1, 2, 3 }, new long[] { 20, 50, 100 }, 1, 3)]
        [DataRow(new int[] { 1, 2 }, new long[] { 3, 2 }, 2, 2)]
        [DataRow(new int[] { 0, 1, 2 }, new long[] { 1, 2, 2 }, 2, 2)]
        public void Can_GenerateHistogramFromSorted_BucketCount(int[] value, long[] count, int bucketSize, int expBucketCount)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramMinDepth("A", "b", new ConstantDepth(bucketSize).GetDepth);
            List<ValueCount> values = new List<ValueCount>();
            for (int i = 0; i < value.Length; i++)
                values.Add(new ValueCount(value[i], count[i]));

            // ACT
            histogram.GenerateHistogramFromSortedGroups(values);

            // ASSERT
            Assert.AreEqual(expBucketCount, histogram.Buckets.Count);
        }

        #endregion
    }
}
