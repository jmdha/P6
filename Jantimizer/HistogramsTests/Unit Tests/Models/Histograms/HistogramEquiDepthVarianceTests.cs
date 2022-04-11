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
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 14.106735979665885)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 1)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 48.979587585033826)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 11998.800041670833)]
        public void Can_GenerateHistogram_Table_StandardDeviation(int[] rows, double expDeviation)
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
                Assert.AreEqual(expDeviation, var.StandardDeviation);
            else
                Assert.Fail();
        }

        [TestMethod]
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 20)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 3)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 40)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 6002.4)]
        public void Can_GenerateHistogram_Table_Mean(int[] rows, double expMean)
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

        [TestMethod]
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 1000)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 10)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 12000)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 719856017.1999998)]
        public void Can_GenerateHistogram_Table_Variance(int[] rows, double expVariance)
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
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 14.106735979665885)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 1)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 48.979587585033826)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 11998.800041670833)]
        public void Can_GenerateHistogram_List_StandardDeviation(int[] rows, double expDeviation)
        {
            // ARRANGE
            IDepthHistogram histogram = new HistogramEquiDepthVariance("A", "b", 5);

            // ACT
            histogram.GenerateHistogram(rows.Cast<IComparable>().ToList());

            // ASSERT
            if (histogram.Buckets[0] is IHistogramBucketVariance var)
                Assert.AreEqual(expDeviation, var.StandardDeviation);
            else
                Assert.Fail();
        }

        [TestMethod]
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 20)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 3)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 40)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 6002.4)]
        public void Can_GenerateHistogram_List_Mean(int[] rows, double expMean)
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

        [TestMethod]
        [DataRow(new int[] { 0, 10, 20, 30, 40 }, 1000)]
        [DataRow(new int[] { 1, 2, 3, 4, 5 }, 10)]
        [DataRow(new int[] { 100, 0, 0, 0, 100 }, 12000)]
        [DataRow(new int[] { 1, 2, 30000, 4, 5 }, 719856017.1999998)]
        public void Can_GenerateHistogram_List_Variance(int[] rows, double expVariance)
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
        [DataRow(new int[] { 1 }, new long[] { 20 }, 10, new double[] { 0, 0 })]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 20, new double[] { 0 })]
        [DataRow(new int[] { 1, 200 }, new long[] { 20, 50 }, 15, new double[] { 0, 132003.3333333333, 0, 0, 0 })]
        [DataRow(new int[] { 1, 5 }, new long[] { 10, 5 }, 3, new double[] { 0, 0, 0, 10.666666666666668, 0 })]
        [DataRow(new int[] { 1, 10, 17, 2, 40 }, new long[] { 5, 20, 60, 2, 10 }, 20, new double[] { 303.75, 183.75, 0, 0, 3387.0588235294135 })]
        public void Can_GenerateHistogramFromSorted_Variance(int[] value, long[] count, int bucketSize, double[] expVariances)
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
        [DataRow(new int[] { 1 }, new long[] { 20 }, 10, new double[] { 0, 0 })]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 20, new double[] { 0 })]
        [DataRow(new int[] { 1, 200 }, new long[] { 20, 50 }, 15, new double[] { 0, 93.8041695353795, 0, 0, 0 })]
        [DataRow(new int[] { 1, 5 }, new long[] { 10, 5 }, 3, new double[] { 0, 0, 0, 1.5986105077709065, 0 })]
        [DataRow(new int[] { 1, 10, 17, 2, 40 }, new long[] { 5, 20, 60, 2, 10 }, 20, new double[] { 3.766629793329841, 2.8613807855648994, 0, 0, 14.079728489046214 })]
        public void Can_GenerateHistogramFromSorted_StandardDeviation(int[] value, long[] count, int bucketSize, double[] expDeviations)
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
                    Assert.AreEqual(expDeviations[i], var.StandardDeviation);
                else
                    Assert.Fail();
            }
        }

        [TestMethod]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 10, new double[] { 1, 1 })]
        [DataRow(new int[] { 1 }, new long[] { 20 }, 20, new double[] { 1 })]
        [DataRow(new int[] { 1, 200 }, new long[] { 20, 50 }, 15, new double[] { 1, 133.66666666666666, 200, 200, 200 })]
        [DataRow(new int[] { 1, 5 }, new long[] { 10, 5 }, 3, new double[] { 1, 1, 1, 3.6666666666666665, 5 })]
        [DataRow(new int[] { 1, 10, 17, 2, 40 }, new long[] { 5, 20, 60, 2, 10 }, 20, new double[] { 7.75, 15.25, 17, 17, 28.764705882352942 })]
        public void Can_GenerateHistogramFromSorted_Mean(int[] value, long[] count, int bucketSize, double[] expMeans)
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
                    Assert.AreEqual(expMeans[i], var.Mean);
                else
                    Assert.Fail();
            }
        }

        #endregion
    }
}
