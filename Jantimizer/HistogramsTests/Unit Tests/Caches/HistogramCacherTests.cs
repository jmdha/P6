using Histograms.Caches;
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
            HistogramEquiDepth histogram = new HistogramEquiDepth(tableName, attributeName, depth);
            Assert.IsNotNull(HistogramCacher.Instance);

            // ACT
            HistogramCacher.Instance.AddToCacheIfNotThere(hash, histogram);

            // ASSERT
            HistogramEquiDepth? returnHistogram = HistogramCacher.Instance.GetValueOrNull(hash) as HistogramEquiDepth;
            Assert.IsNotNull(returnHistogram);
            Assert.AreEqual(tableName, returnHistogram.TableName);
            Assert.AreEqual(attributeName, returnHistogram.AttributeName);
            Assert.AreEqual(depth, returnHistogram.Depth);
        }

        [TestMethod]
        [DataRow("A", "s", 10, "abc")]
        [DataRow("B", "saasada", 1, "abc123bca")]
        public void Can_AddToCacheIfNotThere_HistogramEquiDepthVariance(string tableName, string attributeName, int depth, string hash)
        {
            // ARRANGE
            new HistogramCacher();
            HistogramEquiDepthVariance histogram = new HistogramEquiDepthVariance(tableName, attributeName, depth);
            Assert.IsNotNull(HistogramCacher.Instance);

            // ACT
            HistogramCacher.Instance.AddToCacheIfNotThere(hash, histogram);

            // ASSERT
            HistogramEquiDepthVariance? returnHistogram = HistogramCacher.Instance.GetValueOrNull(hash) as HistogramEquiDepthVariance;
            Assert.IsNotNull(returnHistogram);
            Assert.AreEqual(tableName, returnHistogram.TableName);
            Assert.AreEqual(attributeName, returnHistogram.AttributeName);
            Assert.AreEqual(depth, returnHistogram.Depth);
        }

        [TestMethod]
        public void Can_AddToCacheIfNotThere_NoDuplicate()
        {
            // ARRANGE
            new HistogramCacher();
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", "b", 10);
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
            string[] hash = new string[tableName.Length];
            for (int i = 0; i < tableName.Length; i++)
            {
                histograms[i] = new HistogramEquiDepth(tableName[i], attributeName[i], depth[i]);
                hash[i] = HistogramCacher.Instance.GetCacheKey(new string[] { tableName[i], attributeName[i], columnHashes[i], depth[i].ToString() });
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
                Assert.AreEqual(depth[i], returnHistogram.Depth);
            }
        }

        #endregion

        #region ClearCache

        [TestMethod]
        public void Can_ClearCache()
        {
            // ARRANGE
            new HistogramCacher();
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", "b", 10);
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
            HistogramEquiDepth histogram = new HistogramEquiDepth("A", "b", 10);
            Assert.IsNotNull(HistogramCacher.Instance);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram);

            // ACT
            var result = HistogramCacher.Instance.GetAllCacheItems();

            // ASSERT
            Assert.AreEqual(2, result.Count);
        }

        #endregion

        #region SaveCacheToFile

        [TestMethod]
        public void Can_SaveCacheToFile_HistogramEquiDepth()
        {
            // ARRANGE
            string fileName = "histogram-cache.json";
            if (File.Exists(fileName))
                File.Delete(fileName);

            new HistogramCacher(fileName);
            HistogramEquiDepth histogram1 = new HistogramEquiDepth("A", "b", 10);
            HistogramEquiDepth histogram2 = new HistogramEquiDepth("B", "s", 100);
            Assert.IsNotNull(HistogramCacher.Instance);

            // ACT
            HistogramCacher.Instance.UseCacheFile = false;
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram1);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram2);
            HistogramCacher.Instance.SaveCacheToFile();
            HistogramCacher.Instance.UseCacheFile = true;

            // ASSERT
            var list = JsonSerializer.Deserialize<List<CachedHistogram>>(File.ReadAllText(fileName));
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("A", list[0].TableName);
            Assert.AreEqual("b", list[0].AttributeName);
            Assert.AreEqual(10, list[0].Depth);
            Assert.AreEqual(nameof(HistogramEquiDepth), list[0].TypeName);
            Assert.AreEqual("B", list[1].TableName);
            Assert.AreEqual("s", list[1].AttributeName);
            Assert.AreEqual(100, list[1].Depth);
            Assert.AreEqual(nameof(HistogramEquiDepth), list[1].TypeName);

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [TestMethod]
        public void Can_SaveCacheToFile_HistogramEquiDepthVariance()
        {
            // ARRANGE
            string fileName = "histogram-cache.json";
            if (File.Exists(fileName))
                File.Delete(fileName);

            new HistogramCacher(fileName);
            HistogramEquiDepthVariance histogram1 = new HistogramEquiDepthVariance("A", "b", 10);
            HistogramEquiDepthVariance histogram2 = new HistogramEquiDepthVariance("B", "s", 100);
            Assert.IsNotNull(HistogramCacher.Instance);

            // ACT
            HistogramCacher.Instance.UseCacheFile = false;
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram1);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram2);
            HistogramCacher.Instance.SaveCacheToFile();
            HistogramCacher.Instance.UseCacheFile = true;

            // ASSERT
            var list = JsonSerializer.Deserialize<List<CachedHistogram>>(File.ReadAllText(fileName));
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("A", list[0].TableName);
            Assert.AreEqual("b", list[0].AttributeName);
            Assert.AreEqual(10, list[0].Depth);
            Assert.AreEqual(nameof(HistogramEquiDepthVariance), list[0].TypeName);
            Assert.AreEqual("B", list[1].TableName);
            Assert.AreEqual("s", list[1].AttributeName);
            Assert.AreEqual(100, list[1].Depth);
            Assert.AreEqual(nameof(HistogramEquiDepthVariance), list[1].TypeName);

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [TestMethod]
        public void Can_SaveCacheToFile_Mixed()
        {
            // ARRANGE
            string fileName = "histogram-cache.json";
            if (File.Exists(fileName))
                File.Delete(fileName);

            new HistogramCacher(fileName);
            HistogramEquiDepth histogram1 = new HistogramEquiDepth("A", "b", 10);
            HistogramEquiDepthVariance histogram2 = new HistogramEquiDepthVariance("B", "s", 100);
            Assert.IsNotNull(HistogramCacher.Instance);

            // ACT
            HistogramCacher.Instance.UseCacheFile = false;
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram1);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram2);
            HistogramCacher.Instance.SaveCacheToFile();
            HistogramCacher.Instance.UseCacheFile = true;

            // ASSERT
            var list = JsonSerializer.Deserialize<List<CachedHistogram>>(File.ReadAllText(fileName));
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("A", list[0].TableName);
            Assert.AreEqual("b", list[0].AttributeName);
            Assert.AreEqual(10, list[0].Depth);
            Assert.AreEqual(nameof(HistogramEquiDepth), list[0].TypeName);
            Assert.AreEqual("B", list[1].TableName);
            Assert.AreEqual("s", list[1].AttributeName);
            Assert.AreEqual(100, list[1].Depth);
            Assert.AreEqual(nameof(HistogramEquiDepthVariance), list[1].TypeName);

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        #endregion

        #region SaveCacheToFile

        [TestMethod]
        public void Can_LoadCacheFromFile_HistogramEquiDepth()
        {
            // ARRANGE
            string fileName = "histogram-cache.json";
            if (File.Exists(fileName))
                File.Delete(fileName);

            new HistogramCacher(fileName);
            HistogramEquiDepth histogram1 = new HistogramEquiDepth("A", "b", 10);
            HistogramEquiDepth histogram2 = new HistogramEquiDepth("B", "s", 100);
            Assert.IsNotNull(HistogramCacher.Instance);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram1);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram2);
            Assert.AreEqual(2, HistogramCacher.Instance.GetAllCacheItems().Count);
            HistogramCacher.Instance.ClearCache();
            Assert.AreEqual(0, HistogramCacher.Instance.GetAllCacheItems().Count);

            // ACT
            HistogramCacher.Instance.LoadCacheFromFile();

            // ASSERT
            Assert.AreEqual(2, HistogramCacher.Instance.GetAllCacheItems().Count);
            Assert.IsInstanceOfType(HistogramCacher.Instance.GetValueOrNull("abc1"), typeof(HistogramEquiDepth));
            Assert.IsInstanceOfType(HistogramCacher.Instance.GetValueOrNull("abc2"), typeof(HistogramEquiDepth));

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [TestMethod]
        public void Can_LoadCacheFromFile_HistogramEquiDepthVariance()
        {
            // ARRANGE
            string fileName = "histogram-cache.json";
            if (File.Exists(fileName))
                File.Delete(fileName);

            new HistogramCacher(fileName);
            HistogramEquiDepthVariance histogram1 = new HistogramEquiDepthVariance("A", "b", 10);
            HistogramEquiDepthVariance histogram2 = new HistogramEquiDepthVariance("B", "s", 100);
            Assert.IsNotNull(HistogramCacher.Instance);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram1);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram2);
            Assert.AreEqual(2, HistogramCacher.Instance.GetAllCacheItems().Count);
            HistogramCacher.Instance.ClearCache();
            Assert.AreEqual(0, HistogramCacher.Instance.GetAllCacheItems().Count);

            // ACT
            HistogramCacher.Instance.LoadCacheFromFile();

            // ASSERT
            Assert.AreEqual(2, HistogramCacher.Instance.GetAllCacheItems().Count);
            Assert.IsInstanceOfType(HistogramCacher.Instance.GetValueOrNull("abc1"), typeof(HistogramEquiDepthVariance));
            Assert.IsInstanceOfType(HistogramCacher.Instance.GetValueOrNull("abc2"), typeof(HistogramEquiDepthVariance));

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [TestMethod]
        public void Can_LoadCacheFromFile_Mixed()
        {
            // ARRANGE
            string fileName = "histogram-cache.json";
            if (File.Exists(fileName))
                File.Delete(fileName);

            new HistogramCacher(fileName);
            HistogramEquiDepth histogram1 = new HistogramEquiDepth("A", "b", 10);
            HistogramEquiDepthVariance histogram2 = new HistogramEquiDepthVariance("B", "s", 100);
            Assert.IsNotNull(HistogramCacher.Instance);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc1", histogram1);
            HistogramCacher.Instance.AddToCacheIfNotThere("abc2", histogram2);
            Assert.AreEqual(2, HistogramCacher.Instance.GetAllCacheItems().Count);
            HistogramCacher.Instance.ClearCache();
            Assert.AreEqual(0, HistogramCacher.Instance.GetAllCacheItems().Count);

            // ACT
            HistogramCacher.Instance.LoadCacheFromFile();

            // ASSERT
            Assert.AreEqual(2, HistogramCacher.Instance.GetAllCacheItems().Count);
            Assert.IsInstanceOfType(HistogramCacher.Instance.GetValueOrNull("abc1"), typeof(HistogramEquiDepth));
            Assert.IsInstanceOfType(HistogramCacher.Instance.GetValueOrNull("abc2"), typeof(HistogramEquiDepthVariance));

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        #endregion
    }
}
