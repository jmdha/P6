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
    public class BucketHelperTests
    {
        #region Merge

        [TestMethod]
        public void Can_Merge()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);
            TableAttribute refe2 = new TableAttribute("c", "d");

            ibucket2.AddBucket(refe2, ebucket2);

            // ACT
            var result = BucketHelper.Merge(ibucket1, ibucket2);

            // ASSERT
            Assert.AreEqual(2, result.Buckets.Keys.Count);
            Assert.AreEqual(ibucket1.Buckets[refe1], result.Buckets[refe1]);
            Assert.AreEqual(ibucket2.Buckets[refe2], result.Buckets[refe2]);
        }

        [TestMethod]
        public void Can_Merge_Duplicates()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);
            TableAttribute refe2 = new TableAttribute("a", "b");

            ibucket2.AddBucket(refe2, ebucket2);

            // ACT
            var result = BucketHelper.Merge(ibucket1, ibucket2);

            // ASSERT
            Assert.AreEqual(1, result.Buckets.Keys.Count);
            Assert.AreEqual(ibucket1.Buckets[refe1], result.Buckets[refe1]);
        }

        #endregion

        #region MergeOnOverlap

        [TestMethod]
        public void Can_MergeOnOverlap_None()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);
            TableAttribute refe2 = new TableAttribute("c", "d");

            ibucket2.AddBucket(refe2, ebucket2);

            // ACT
            var result = BucketHelper.MergeOnOverlap(
                new List<IntermediateBucket>() { ibucket1 },
                new List<IntermediateBucket>() { ibucket2 });

            // ASSERT
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Can_MergeOnOverlap_Single()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            TableAttribute refe2 = new TableAttribute("a", "b");

            ibucket2.AddBucket(refe2, ebucket1);

            // ACT
            var result = BucketHelper.MergeOnOverlap(
                new List<IntermediateBucket>() { ibucket1 },
                new List<IntermediateBucket>() { ibucket2 });

            // ASSERT
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void Can_MergeOnOverlap_Multiple()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 1);
            TableAttribute refe2 = new TableAttribute("c", "d");

            ibucket1.AddBucket(refe1, ebucket1);
            ibucket1.AddBucket(refe2, ebucket2);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            TableAttribute refe3 = new TableAttribute("a", "b");
            TableAttribute refe4 = new TableAttribute("c", "d");

            ibucket2.AddBucket(refe3, ebucket1);
            ibucket2.AddBucket(refe4, ebucket2);

            // ACT
            var result = BucketHelper.MergeOnOverlap(
                new List<IntermediateBucket>() { ibucket1 },
                new List<IntermediateBucket>() { ibucket2 });

            // ASSERT
            Assert.AreEqual(2, result.Count);
        }

        #endregion

        #region DoesOverlap

        [TestMethod]
        public void Can_DoesOverlap_None_1()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);

            ibucket2.AddBucket(refe1, ebucket2);

            // ACT
            var result = BucketHelper.DoesOverlap(refe1, ibucket1, ibucket2);

            // ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Can_DoesOverlap_None_2()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);
            TableAttribute refe2 = new TableAttribute("b", "c");

            ibucket2.AddBucket(refe2, ebucket2);

            // ACT
            var result = BucketHelper.DoesOverlap(refe1, ibucket1, ibucket2);

            // ASSERT
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Can_DoesOverlap_Overlap()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);
            TableAttribute refe2 = new TableAttribute("a", "b");

            ibucket2.AddBucket(refe2, ebucket1);

            // ACT
            var result = BucketHelper.DoesOverlap(refe1, ibucket1, ibucket2);

            // ASSERT
            Assert.IsTrue(result);
        }

        #endregion
    }
}
