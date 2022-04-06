using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Caches;
using ToolsTests.Stubs;

namespace ToolsTests.Caches
{
    [TestClass]
    public class BaseCacherServiceTests
    {
        #region GetCacheKey

        [TestMethod]
        [DataRow(new string[] { "a", "b" }, "~�Ca\"��/@�+���")]
        [DataRow(new string[] { "a", "b", "c" }, "�P�<�O�֖?}(�r")]
        [DataRow(new string[] { "a", "bqrr32591" }, "�XjC�Y����1��")]
        public void Can_GetCacheKey(string[] values, string expected)
        {
            // ARRANGE
            ICacherService<int?> service = new TestBaseCacherService();

            // ACT
            var result = service.GetCacheKey(values);

            // ASSERT
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region GetValueOrNull

        [TestMethod]
        [DataRow(new string[] { "a", "b" }, 1)]
        [DataRow(new string[] { "a", "b", "c" }, 50)]
        [DataRow(new string[] { "a", "bqrr32591" }, 99999)]
        public void Can_GetValueOrNull(string[] hashValues, int insert)
        {
            // ARRANGE
            ICacherService<int?> service = new TestBaseCacherService();
            service.AddToCacheIfNotThere(hashValues, insert);

            // ACT
            var result = service.GetValueOrNull(hashValues);

            // ASSERT
            Assert.AreEqual(insert, result);
        }

        #endregion

        #region AddToCacheIfNotThere

        [TestMethod]
        [DataRow(new string[] { "a", "b" }, 1)]
        [DataRow(new string[] { "a", "b", "c" }, 50)]
        [DataRow(new string[] { "a", "bqrr32591" }, 99999)]
        public void Can_AddToCacheIfNotThere(string[] hashValues, int insert)
        {
            // ARRANGE
            ICacherService<int?> service = new TestBaseCacherService();

            // ACT
            service.AddToCacheIfNotThere(hashValues, insert);

            // ASSERT
            var result = service.GetValueOrNull(hashValues);
            Assert.AreEqual(insert, result);
        }

        #endregion
    }
}
