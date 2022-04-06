using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryPlanParser.Caches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QueryPlanParserTests.Unit_Tests.Caches
{
    [TestClass]
    public class QueryPlanCacherTests
    {
        [TestMethod]
        public void Can_MakeNewInstance()
        {
            // ARRANGE
            string cacheFile = "query-plan-cache.json";

            // ACT
            new QueryPlanCacher(cacheFile);

            // ASSERT
            Assert.IsNotNull(QueryPlanCacher.Instance);
            Assert.AreEqual(cacheFile, QueryPlanCacher.Instance.CacheFile.Name);
        }

        #region AddToCacheIfNotThere

        [TestMethod]
        [DataRow(10u, "abc")]
        [DataRow(1u, "abc123bca")]
        public void Can_AddToCacheIfNotThere(ulong value, string hash)
        {
            // ARRANGE
            new QueryPlanCacher();
            Assert.IsNotNull(QueryPlanCacher.Instance);

            // ACT
            QueryPlanCacher.Instance.AddToCacheIfNotThere(hash, value);

            // ASSERT
            ulong? returnValue = QueryPlanCacher.Instance.GetValueOrNull(hash);
            Assert.IsNotNull(returnValue);
            Assert.AreEqual(value, returnValue);
        }

        [TestMethod]
        public void Can_AddToCacheIfNotThere_NoDuplicate()
        {
            // ARRANGE
            ulong expected = 1;
            ulong unexpected = 10;
            new QueryPlanCacher();
            Assert.IsNotNull(QueryPlanCacher.Instance);

            // ACT
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc", expected);
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc", unexpected);

            // ASSERT
            var allHistos = QueryPlanCacher.Instance.GetAllCacheItems();
            Assert.AreEqual(expected, ulong.Parse(allHistos[0].Content));
        }

        #endregion

        #region ClearCache

        [TestMethod]
        public void Can_ClearCache()
        {
            // ARRANGE
            new QueryPlanCacher();
            Assert.IsNotNull(QueryPlanCacher.Instance);
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc1", 10);
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc2", 20);

            // ACT
            Assert.AreEqual(2, QueryPlanCacher.Instance.GetAllCacheItems().Count);
            QueryPlanCacher.Instance.ClearCache();

            // ASSERT
            Assert.AreEqual(0, QueryPlanCacher.Instance.GetAllCacheItems().Count);
        }

        #endregion

        #region GetAllCacheItems

        [TestMethod]
        public void Can_GetAllCacheItems()
        {
            // ARRANGE
            ulong expected1 = 1;
            ulong expected2 = 10;
            new QueryPlanCacher();
            Assert.IsNotNull(QueryPlanCacher.Instance);
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc1", expected1);
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc2", expected2);

            // ACT
            var result = QueryPlanCacher.Instance.GetAllCacheItems();

            // ASSERT
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(expected1, ulong.Parse(result[0].Content));
        }

        #endregion

        #region SaveCacheToFile

        [TestMethod]
        public void Can_SaveCacheToFile_HistogramEquiDepth()
        {
            // ARRANGE
            string fileName = "query-plan-cache.json";
            if (File.Exists(fileName))
                File.Delete(fileName);

            ulong expected1 = 1;
            ulong expected2 = 10;
            new QueryPlanCacher(fileName);
            Assert.IsNotNull(QueryPlanCacher.Instance);

            // ACT
            QueryPlanCacher.Instance.UseCacheFile = false;
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc1", expected1);
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc2", expected2);
            QueryPlanCacher.Instance.SaveCacheToFile();
            QueryPlanCacher.Instance.UseCacheFile = true;

            // ASSERT
            var list = JsonSerializer.Deserialize<Dictionary<string, ulong>>(File.ReadAllText(fileName));
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(expected1, list["abc1"]);
            Assert.AreEqual(expected2, list["abc2"]);

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        #endregion

        #region SaveCacheToFile

        [TestMethod]
        public void Can_LoadCacheFromFile_HistogramEquiDepth()
        {
            // ARRANGE
            string fileName = "query-plan-cache.json";
            if (File.Exists(fileName))
                File.Delete(fileName);

            ulong expected1 = 1;
            ulong expected2 = 10;
            new QueryPlanCacher(fileName);
            Assert.IsNotNull(QueryPlanCacher.Instance);
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc1", expected1);
            QueryPlanCacher.Instance.AddToCacheIfNotThere("abc2", expected2);
            Assert.AreEqual(2, QueryPlanCacher.Instance.GetAllCacheItems().Count);
            QueryPlanCacher.Instance.ClearCache();
            Assert.AreEqual(0, QueryPlanCacher.Instance.GetAllCacheItems().Count);

            // ACT
            QueryPlanCacher.Instance.LoadCacheFromFile();

            // ASSERT
            Assert.AreEqual(2, QueryPlanCacher.Instance.GetAllCacheItems().Count);
            Assert.AreEqual(expected1, QueryPlanCacher.Instance.GetValueOrNull("abc1"));
            Assert.AreEqual(expected2, QueryPlanCacher.Instance.GetValueOrNull("abc2"));

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        #endregion
    }
}
