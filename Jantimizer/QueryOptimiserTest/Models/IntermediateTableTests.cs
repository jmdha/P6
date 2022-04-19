using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest.Models
{
    [TestClass]
    public class IntermediateTableTests
    {
        #region Constuctor

        [TestMethod]
        public void Can_Generate_Constructor_Simple()
        {
            // ARRANGE

            // ACT
            var table = new IntermediateTable();

            // ASSERT
            Assert.IsNotNull(table.Buckets);
            Assert.IsNotNull(table.References);
        }

        [TestMethod]
        public void Can_Generate_Constructor_Advanced()
        {
            // ARRANGE
            List<IntermediateBucket> buckets = new List<IntermediateBucket>() {
                new IntermediateBucket(),
                new IntermediateBucket()
            };
            List<TableAttribute> references = new List<TableAttribute>() { 
                new TableAttribute("a", "b"),
                new TableAttribute("c", "d")
            };

            // ACT
            var table = new IntermediateTable(buckets, references);

            // ASSERT
            Assert.IsNotNull(table.Buckets);
            Assert.IsNotNull(table.References);
            foreach (var bucket in table.Buckets)
                Assert.IsTrue(buckets.Contains(bucket));
            foreach (var reference in table.References)
                Assert.IsTrue(references.Contains(reference));
        }

        #endregion

        #region GetRowEstimate

        [TestMethod]
        [DataRow(1, 2, 3)]
        [DataRow(10, 2, 12)]
        [DataRow(0, 500, 500)]
        public void Can_GetRowEstimate_TwoBuckets(long b1Est, long b2Est, long expEst)
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, b1Est);
            TableAttribute refe1 = new TableAttribute("a", "b");
            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, b2Est);
            TableAttribute refe2 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);
            ibucket2.AddBucket(refe2, ebucket2);


            List<IntermediateBucket> buckets = new List<IntermediateBucket>() {
                ibucket1,
                ibucket2
            };
            List<TableAttribute> references = new List<TableAttribute>() {
                refe1,
                refe2
            };
            var table = new IntermediateTable(buckets, references);

            // ACT
            var result = table.GetRowEstimate();

            // ASSERT
            Assert.AreEqual(expEst, result);
        }

        [TestMethod]
        [DataRow(1, 2, 3, 4, 14)]
        [DataRow(10, 2, 5, 100, 520)]
        [DataRow(0, 500, 5, 0, 0)]
        [DataRow(0, 500, 5, 1, 5)]
        public void Can_GetRowEstimate_FourBuckets(long b1Est, long b2Est, long b3Est, long b4Est, long expEst)
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, b1Est);
            BucketEstimate ebucket2 = new BucketEstimate(bucket1, b2Est);
            TableAttribute refe1 = new TableAttribute("a", "b");
            TableAttribute refe2 = new TableAttribute("c", "d");
            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket3 = new BucketEstimate(bucket2, b3Est);
            BucketEstimate ebucket4 = new BucketEstimate(bucket2, b4Est);
            TableAttribute refe3 = new TableAttribute("e", "f");
            TableAttribute refe4 = new TableAttribute("g", "h");

            ibucket1.AddBucket(refe1, ebucket1);
            ibucket1.AddBucket(refe2, ebucket2);
            ibucket2.AddBucket(refe3, ebucket3);
            ibucket2.AddBucket(refe4, ebucket4);

            List<IntermediateBucket> buckets = new List<IntermediateBucket>() {
                ibucket1,
                ibucket2
            };
            List<TableAttribute> references = new List<TableAttribute>() {
                refe1,
                refe2,
                refe3,
                refe4
            };
            var table = new IntermediateTable(buckets, references);

            // ACT
            var result = table.GetRowEstimate();

            // ASSERT
            Assert.AreEqual(expEst, result);
        }

        #endregion

        #region GetBuckets

        [TestMethod]
        public void Can_GetBuckets_Single()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucket(refe1, ebucket1);


            List<IntermediateBucket> buckets = new List<IntermediateBucket>() {
                ibucket1
            };
            List<TableAttribute> references = new List<TableAttribute>() {
                refe1
            };
            var table = new IntermediateTable(buckets, references);

            // ACT
            var result = table.GetBuckets(refe1);

            // ASSERT
            Assert.AreEqual(bucket1, result[0]);
        }

        [TestMethod]
        public void Can_GetBuckets_Multiple()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");
            TableAttribute refe2 = new TableAttribute("c", "d");

            ibucket1.AddBucket(refe1, ebucket1);
            ibucket1.AddBucket(refe2, ebucket2);

            List<IntermediateBucket> buckets = new List<IntermediateBucket>() {
                ibucket1
            };
            List<TableAttribute> references = new List<TableAttribute>() {
                refe1,
                refe2
            };
            var table = new IntermediateTable(buckets, references);

            // ACT
            var result1 = table.GetBuckets(refe1);
            var result2 = table.GetBuckets(refe2);

            // ASSERT
            Assert.AreEqual(bucket1, result1[0]);
            Assert.AreEqual(bucket2, result2[0]);
        }

        #endregion
    }
}
