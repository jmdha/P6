using Histograms;
using Histograms.Caches;
using Histograms.DepthCalculators;
using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HistogramsTests.Unit_Tests.Caches
{
    [TestClass]
    public class HistogramCacherTests
    {
        [TestMethod]
        public void Can_MakeNewInstance()
        {
            // ARRANGE
            string cacheFile = "histogram-cache.json";

            // ACT
            new HistogramCacher(cacheFile);

            // ASSERT
            Assert.IsNotNull(HistogramCacher.Instance);
            Assert.AreEqual(cacheFile, HistogramCacher.Instance.CacheFile.Name);
        }

        #region AddToCacheIfNotThere

        [TestMethod]
        [DataRow("A", "s", 10, "abc")]
        [DataRow("B", "saasada", 1, "abc123bca")]
        public void Can_AddToCacheIfNotThere_HistogramEquiDepth(string tableName, string attributeName, int depth, string hash)
        {
            // ARRANGE
            new HistogramCacher();
            DepthCalculator getDepth = new ConstantDepth(depth).GetDepth;
            HistogramEquiDepth histogram = new HistogramEquiDepth(tableName, attributeName, getDepth);
            Assert.IsNotNull(HistogramCacher.Instance);

            // ACT
            HistogramCacher.Instance.AddToCacheIfNotThere(hash, histogram);

            // ASSERT
            HistogramEquiDepth? returnHistogram = HistogramCacher.Instance.GetValueOrNull(hash) as HistogramEquiDepth;
            Assert.IsNotNull(returnHistogram);
            Assert.AreEqual(tableName, returnHistogram.TableName);
            Assert.AreEqual(attributeName, returnHistogram.AttributeName);
            Assert.AreEqual(getDepth, returnHistogram.GetDepth);
        }

        [TestMethod]
        [DataRow("A", "s", 10, "abc")]
        [DataRow("B", "saasada", 1, "abc123bca")]
        public void Can_AddToCacheIfNotThere_HistogramEquiDepthVariance(string tableName, string attributeName, int depth, string hash)
        {
            // ARRANGE
            new HistogramCacher();
            DepthCalculator getDepth = new ConstantDepth(depth).GetDepth;
            HistogramEquiDepthVariance histogram = new HistogramEquiDepthVariance(tableName, attributeName, getDepth);
            Assert.IsNotNull(HistogramCacher.Instance);

            // ACT
            HistogramCacher.Instance.AddToCacheIfNotThere(hash, histogram);

            // ASSERT
            HistogramEquiDepthVariance? returnHistogram = HistogramCacher.Instance.GetValueOrNull(hash) as HistogramEquiDepthVariance;
            Assert.IsNotNull(returnHistogram);
            Assert.AreEqual(tableName, returnHistogram.TableName);
            Assert.AreEqual(attributeName, returnHistogram.AttributeName);
            Assert.AreEqual(getDepth, returnHistogram.GetDepth);
        }

        [TestMethod]
        public void Can_AddToCacheIfNotThere_NoDuplicate()
        {
            // ARRANGE
            new HistogramCacher();
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", "b", new ConstantDepth(10).GetDepth);
            Assert.IsNotNull(HistogramCacher.Instance);

            // ACT
            HistogramCacher.Instance.AddToCacheIfNotThere("abc", histogram);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc", histogram);

            // ASSERT
            var allHistos = HistogramCacher.Instance.GetAllCacheItems();
            Assert.AreEqual(1, allHistos.Count);
        }

        [TestMethod]
        [DataRow(
            new string[] { "A", "A", "A"},
            new string[] { "s", "s", "s"},
            new int[] { 1, 10, 93839 },
            new string[] { "abc", "abc", "abc" })]
        public void Can_AddToCacheIfNotThere_DifferenBucketSizes(string[] tableName, string[] attributeName, int[] depth, string[] columnHashes)
        {
            // ARRANGE
            new HistogramCacher();
            Assert.IsNotNull(HistogramCacher.Instance);
            HistogramEquiDepth[] histograms = new HistogramEquiDepth[tableName.Length];
            DepthCalculator[] depthCalculators = depth.Select(x => (DepthCalculator)new ConstantDepth(x).GetDepth).ToArray();
            string[] hash = new string[tableName.Length];
            for (int i = 0; i < tableName.Length; i++)
            {
                histograms[i] = new HistogramEquiDepth(tableName[i], attributeName[i], depthCalculators[i]);
                hash[i] = HistogramCacher.Instance.GetCacheKey(new string[] { tableName[i], attributeName[i], columnHashes[i], depthCalculators[i].GetHashCode().ToString() });
            }

            // ACT
            for (int i = 0; i < tableName.Length; i++)
                HistogramCacher.Instance.AddToCacheIfNotThere(hash[i], histograms[i]);

            // ASSERT
            for (int i = 0; i < tableName.Length; i++)
            {
                HistogramEquiDepth? returnHistogram = HistogramCacher.Instance.GetValueOrNull(hash[i]) as HistogramEquiDepth;
                Assert.IsNotNull(returnHistogram);
                Assert.AreEqual(tableName[i], returnHistogram.TableName);
                Assert.AreEqual(attributeName[i], returnHistogram.AttributeName);
                Assert.AreEqual(depthCalculators[i], returnHistogram.GetDepth);
            }
        }

        #endregion

        #region ClearCache

        [TestMethod]
        public void Can_ClearCache()
        {
            // ARRANGE
            new HistogramCacher();
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", "b", new ConstantDepth(10).GetDepth);
            Assert.IsNotNull(HistogramCacher.Instance);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram);

            // ACT
            Assert.AreEqual(2, HistogramCacher.Instance.GetAllCacheItems().Count);
            HistogramCacher.Instance.ClearCache();

            // ASSERT
            Assert.AreEqual(0, HistogramCacher.Instance.GetAllCacheItems().Count);
        }

        #endregion

        #region GetAllCacheItems

        [TestMethod]
        public void Can_GetAllCacheItems()
        {
            // ARRANGE
            new HistogramCacher();
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", "b", new ConstantDepth(10).GetDepth);
            Assert.IsNotNull(HistogramCacher.Instance);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram);

            // ACT
            var result = HistogramCacher.Instance.GetAllCacheItems();

            // ASSERT
            Assert.AreEqual(2, result.Count);
        }

        #endregion
    }
}
