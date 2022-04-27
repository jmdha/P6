using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest.Models
{
    [TestClass]
    public class IntermediateBucketTests
    {
        #region AddBucket

        [TestMethod]
        [DataRow("a", "b", 0, 1, 10, 5)]
        [DataRow("a", "a", 0, 1, 10, -1)]
        [DataRow("aAVBB", "_", 10, 11, 1, 35415315)]
        public void Can_AddBucket(string table, string attribute, int valueStart, int valueEnd, int count, long estimate)
        {
            // ARRANGE
            IntermediateBucket ibucket = new IntermediateBucket();
            IHistogramBucket bucket = new HistogramBucket(valueStart, valueEnd, count);
            BucketEstimate ebucket = new BucketEstimate(bucket, estimate);
            TableAttribute refe = new TableAttribute(table, attribute);

            // ACT
            ibucket.AddBucketIfNotThere(refe, ebucket);

            // ASSERT
            Assert.AreEqual(ebucket, ibucket.Buckets[refe]);
        }

        #endregion

        #region Constructor

        [TestMethod]
        public void Can_Constructor_AddBuckets()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");

            ibucket1.AddBucketIfNotThere(refe1, ebucket1);

            IntermediateBucket ibucket2 = new IntermediateBucket();
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);
            TableAttribute refe2 = new TableAttribute("c", "d");

            ibucket2.AddBucketIfNotThere(refe2, ebucket2);

            // ACT
            var newIBucket = new IntermediateBucket(ibucket1, ibucket2);

            // ASSERT
            Assert.AreEqual(2, newIBucket.Buckets.Keys.Count);
            Assert.AreEqual(ebucket1, newIBucket.Buckets[refe1]);
            Assert.AreEqual(ebucket2, newIBucket.Buckets[refe2]);
        }

        #endregion

        #region AddBuckets

        [TestMethod]
        public void Can_AddBuckets_NoDuplicates()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);
            TableAttribute refe2 = new TableAttribute("c", "d");

            ibucket1.AddBucketIfNotThere(refe1, ebucket1);
            ibucket1.AddBucketIfNotThere(refe2, ebucket2);

            IntermediateBucket ibucket2 = new IntermediateBucket();

            // ACT
            ibucket2.AddBucketsIfNotThere(ibucket1);

            // ASSERT
            Assert.AreEqual(2, ibucket1.Buckets.Keys.Count);
            Assert.AreEqual(ebucket1, ibucket1.Buckets[refe1]);
            Assert.AreEqual(ebucket2, ibucket1.Buckets[refe2]);
        }

        [TestMethod]
        public void Can_AddBuckets_Duplicates()
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, 1);
            TableAttribute refe1 = new TableAttribute("a", "b");
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, 2);
            TableAttribute refe2 = new TableAttribute("a", "b");

            ibucket1.AddBucketIfNotThere(refe1, ebucket1);
            ibucket1.AddBucketIfNotThere(refe2, ebucket2);

            IntermediateBucket ibucket2 = new IntermediateBucket();

            // ACT
            ibucket2.AddBucketsIfNotThere(ibucket1);

            // ASSERT
            Assert.AreEqual(1, ibucket1.Buckets.Keys.Count);
        }

        #endregion

        #region GetEstimateOfAllBuckets

        [TestMethod]
        [DataRow(1, 2, 2)]
        [DataRow(2, 2, 4)]
        [DataRow(2, 10, 20)]
        public void Cant_GetEstimateOfAllBuckets(long est1, long est2, long expEst)
        {
            // ARRANGE
            IntermediateBucket ibucket1 = new IntermediateBucket();
            IHistogramBucket bucket1 = new HistogramBucket(0, 1, 2);
            BucketEstimate ebucket1 = new BucketEstimate(bucket1, est1);
            TableAttribute refe1 = new TableAttribute("a", "b");
            IHistogramBucket bucket2 = new HistogramBucket(1, 2, 3);
            BucketEstimate ebucket2 = new BucketEstimate(bucket2, est2);
            TableAttribute refe2 = new TableAttribute("c", "d");

            ibucket1.AddBucketIfNotThere(refe1, ebucket1);
            ibucket1.AddBucketIfNotThere(refe2, ebucket2);

            // ACT
            var result = ibucket1.RowEstimate;

            // ASSERT
            Assert.AreEqual(expEst, result);
        }

        #endregion
    }
}
