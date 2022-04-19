using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Helpers;
using QueryOptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest.Helpers
{
    [TestClass]
    public class TableHelperTests
    {
        #region Join

        [TestMethod]
        public void Can_Join_WithoutOverlap()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            var table1 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe1 }
                );

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 1);
            TableAttribute refe2 = new TableAttribute("b", "b");

            ibucket2.AddBucket(refe2, ebucket2);

            var table2 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket2 },
                new List<TableAttribute>() { refe2 }
                );

            // ACT
            var result = TableHelper.Join(table1, table2);

            // ASSERT
            Assert.AreEqual(2, result.References.Count);
            Assert.AreEqual(1, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_Join_WithOverlap_1()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            var table1 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe1 }
                );

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 1);
            TableAttribute refe2 = new TableAttribute("a", "b");

            ibucket2.AddBucket(refe2, ebucket2);

            var table2 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket2 },
                new List<TableAttribute>() { refe2 }
                );

            // ACT
            var result = TableHelper.Join(table1, table2);

            // ASSERT
            Assert.AreEqual(1, result.References.Count);
            Assert.AreEqual(0, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_Join_WithOverlap_2()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            var table1 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe1 }
                );

            IntermediateBucket ibucket2 = new IntermediateBucket();
            TableAttribute refe2 = new TableAttribute("a", "b");

            ibucket2.AddBucket(refe2, ebucket1);

            var table2 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe2 }
                );

            // ACT
            var result = TableHelper.Join(table1, table2);

            // ASSERT
            Assert.AreEqual(1, result.References.Count);
            Assert.AreEqual(1, result.Buckets.Count);
        }

        #endregion

        #region JoinWithOverlap

        [TestMethod]
        public void Can_JoinWithOverlap_1()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            var table1 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe1 }
                );

            IntermediateBucket ibucket2 = new IntermediateBucket();
            TableAttribute refe2 = new TableAttribute("b", "b");

            ibucket2.AddBucket(refe2, ebucket1);

            var table2 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe2 }
                );

            var table3 = new IntermediateTable(
                new List<IntermediateBucket>() {  },
                new List<TableAttribute>() {  }
                );

            // ACT
            var result = TableHelper.JoinWithOverlap(table3, table1, table2, refe1);

            // ASSERT
            Assert.AreEqual(1, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_JoinWithOverlap_2()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            var table1 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe1 }
                );

            IntermediateBucket ibucket2 = new IntermediateBucket();
            TableAttribute refe2 = new TableAttribute("a", "b");

            ibucket2.AddBucket(refe2, ebucket1);

            var table2 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe2 }
                );

            var table3 = new IntermediateTable(
                new List<IntermediateBucket>() { },
                new List<TableAttribute>() { }
                );

            // ACT
            var result = TableHelper.JoinWithOverlap(table3, table1, table2, refe1);

            // ASSERT
            Assert.AreEqual(1, result.Buckets.Count);
        }

        [TestMethod]
        public void Can_JoinWithOverlap_3()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            var table1 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe1 }
                );

            IntermediateBucket ibucket2 = new IntermediateBucket();
            TableAttribute refe2 = new TableAttribute("b", "b");

            ibucket2.AddBucket(refe2, ebucket1);

            var table2 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe2 }
                );

            var table3 = new IntermediateTable(
                new List<IntermediateBucket>() { },
                new List<TableAttribute>() { }
                );

            // ACT
            var result = TableHelper.JoinWithOverlap(table3, table1, table2, new TableAttribute("b", "q"));

            // ASSERT
            Assert.AreEqual(0, result.Buckets.Count);
        }

        #endregion

        #region JoinWithoutOverlap

        [TestMethod]
        public void Can_JoinWithoutOverlap_1()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            var table1 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket1 },
                new List<TableAttribute>() { refe1 }
                );

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 1);
            TableAttribute refe2 = new TableAttribute("b", "b");

            ibucket2.AddBucket(refe2, ebucket2);

            var table2 = new IntermediateTable(
                new List<IntermediateBucket>() { ibucket2 },
                new List<TableAttribute>() { refe2 }
                );

            var table3 = new IntermediateTable(
                new List<IntermediateBucket>() { },
                new List<TableAttribute>() { }
                );

            // ACT
            var result = TableHelper.JoinWithoutOverlap(table3, table1, table2);

            // ASSERT
            Assert.AreEqual(1, result.Buckets.Count);
        }

        #endregion
    }
}
