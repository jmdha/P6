using Histograms.Caches;
using Histograms.Models;
using HistogramsTests.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.Caches
{
    [TestClass]
    public class CachedHistogramTests
    {
        [TestMethod]
        [DataRow("A", "s", 10, "adafaf30")]
        [DataRow("B", "ssss", 1, "adasfa021")]
        public void Constructor_CanSetWith_HistogramEquiDepth(string tableName, string attributeName, int depth, string hash)
        {
            // ARRANGE
            HistogramEquiDepth histogram = new HistogramEquiDepth(tableName, attributeName, depth);
            List<IHistogramBucket> buckets = new List<IHistogramBucket>();
            buckets.Add(new HistogramBucket(0, 1, 1));
            buckets.Add(new HistogramBucket(0, 1, 1));
            buckets.Add(new HistogramBucket(0, 1, 1));
            histogram.Buckets.AddRange(buckets);

            // ACT
            CachedHistogram cachedHistogram = new CachedHistogram(histogram, hash);

            // ASSERT
            Assert.AreEqual(tableName, cachedHistogram.TableName);
            Assert.AreEqual(attributeName, cachedHistogram.AttributeName);
            Assert.AreEqual(depth, cachedHistogram.Depth);
            Assert.AreEqual(hash, cachedHistogram.Hash);
            Assert.AreEqual(nameof(HistogramBucket), cachedHistogram.Buckets[0].TypeName);
            Assert.AreEqual(nameof(HistogramEquiDepth), cachedHistogram.TypeName);
        }

        [TestMethod]
        [DataRow("A", "s", 10, "adafaf30")]
        [DataRow("B", "ssss", 1, "adasfa021")]
        public void Constructor_CanSetWith_HistogramEquiDepthVariance(string tableName, string attributeName, int depth, string hash)
        {
            // ARRANGE
            HistogramEquiDepthVariance histogram = new HistogramEquiDepthVariance(tableName, attributeName, depth);
            List<IHistogramBucket> buckets = new List<IHistogramBucket>();
            buckets.Add(new HistogramBucketVariance(0, 1, 1, 0, 0, 0));
            buckets.Add(new HistogramBucketVariance(0, 1, 1, 0, 0, 0));
            buckets.Add(new HistogramBucketVariance(0, 1, 1, 0, 0, 0));
            histogram.Buckets.AddRange(buckets);

            // ACT
            CachedHistogram cachedHistogram = new CachedHistogram(histogram, hash);

            // ASSERT
            Assert.AreEqual(tableName, cachedHistogram.TableName);
            Assert.AreEqual(attributeName, cachedHistogram.AttributeName);
            Assert.AreEqual(depth, cachedHistogram.Depth);
            Assert.AreEqual(hash, cachedHistogram.Hash);
            Assert.AreEqual(nameof(HistogramBucketVariance), cachedHistogram.Buckets[0].TypeName);
            Assert.AreEqual(nameof(HistogramEquiDepthVariance), cachedHistogram.TypeName);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Constructor_ThrowsIfUnknownHistogramType()
        {
            new CachedHistogram(new BadHistogram(), "");
        }
    }
}
