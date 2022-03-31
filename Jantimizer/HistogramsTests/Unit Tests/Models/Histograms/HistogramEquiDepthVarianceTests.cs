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

        #region GenerateHistogramFromSorted

        [TestMethod]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 10, 2)]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 5, 4)]
        [DataRow(new int[] { 1, 2 }, new long[] { 20, 50 }, 10, 7)]
        [DataRow(new int[] { 1, 2 }, new long[] { 20, 50 }, 5, 14)]
        [DataRow(new int[] { 1, 2, 3 }, new long[] { 20, 50, 100 }, 10, 17)]
        [DataRow(new int[] { 1, 2, 3 }, new long[] { 20, 50, 100 }, 1, 170)]
        public void Can_GenerateHistogramFromSorted_BucketCount(int[] value, long[] count, int bucketSize, int expBucketCount)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", bucketSize);
            List<ValueCount> values = new List<ValueCount>();
            for (int i = 0; i < value.Length; i++)
                values.Add(new ValueCount(value[i], count[i]));

            // ACT
            histogram.GenerateHistogramFromSortedGroups(values);

            // ASSERT
            Assert.AreEqual(expBucketCount, histogram.Buckets.Count);
        }

        [TestMethod]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 10, new int[] { 0, 0 })]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 20, new int[] { 0 })]
        [DataRow(new int[] { 1, 200 }, new long[] { 20, 50 }, 15, new int[] { 0, 93, 0, 0, 0 })]
        [DataRow(new int[] { 1, 5 }, new long[] { 10, 5 }, 3, new int[] { 0, 0, 0, 1, 0 })]
        [DataRow(new int[] { 1, 10, 17, 2, 40 }, new long[] { 5, 20, 60, 2, 10 }, 20, new int[] { 3, 2, 0, 0, 14 })]
        public void Can_GenerateHistogramFromSorted_Variance(int[] value, long[] count, int bucketSize, int[] expVariances)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", bucketSize);
            List<ValueCount> values = new List<ValueCount>();
            for (int i = 0; i < value.Length; i++)
                values.Add(new ValueCount(value[i], count[i]));

            // ACT
            histogram.GenerateHistogramFromSortedGroups(values);

            // ASSERT
            Assert.IsTrue(histogram.Buckets.Count > 0);
            for (int i = 0; i < histogram.Buckets.Count; i++)
            {
                if (histogram.Buckets[i] is IHistogramBucketVariance var)
                    Assert.AreEqual(expVariances[i], var.Variance);
                else
                    Assert.Fail();
            }
        }

        [TestMethod]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 10, new int[] { 1, 1 })]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 20, new int[] { 1 })]
        [DataRow(new int[] { 1, 200 }, new long[] { 20, 50 }, 15, new int[] { 1, 133, 200, 200, 200 })]
        [DataRow(new int[] { 1, 5 }, new long[] { 10, 5 }, 3, new int[] { 1, 1, 1, 3, 5 })]
        [DataRow(new int[] { 1, 10, 17, 2, 40 }, new long[] { 5, 20, 60, 2, 10 }, 20, new int[] { 7, 15, 17, 17, 28 })]
        public void Can_GenerateHistogramFromSorted_Mean(int[] value, long[] count, int bucketSize, int[] expVariances)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", bucketSize);
            List<ValueCount> values = new List<ValueCount>();
            for (int i = 0; i < value.Length; i++)
                values.Add(new ValueCount(value[i], count[i]));

            // ACT
            histogram.GenerateHistogramFromSortedGroups(values);

            // ASSERT
            Assert.IsTrue(histogram.Buckets.Count > 0);
            for (int i = 0; i < histogram.Buckets.Count; i++)
            {
                if (histogram.Buckets[i] is IHistogramBucketVariance var)
                    Assert.AreEqual(expVariances[i], var.Mean);
                else
                    Assert.Fail();
            }
        }

        #endregion
    }
}
