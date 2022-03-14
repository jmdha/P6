using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryPlanParser.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryPlanParserTests.UnitTests.Parsers
{
    [TestClass]
    public class BasePlanParserTests
    {
        #region TimeSpanFromMs
        [TestMethod]
        [DataRow("1000.0","00:00:01")]
        [DataRow("100.0","00:00:00.1000000")]
        [DataRow("10.0", "00:00:00.0100000")]
        [DataRow("10000.0","00:00:10")]
        [DataRow("999000.0","00:16:39")]
        [DataRow("-1.0", "-00:00:00.0010000")]
        public void Can_TimeSpanFromMs(string decimalValue, string expectedTimespan)
        {
            // ARRANGE
            // ACT
            var result = BasePlanParser.TimeSpanFromMs(decimal.Parse(decimalValue));

            // ASSERT
            Assert.AreEqual(expectedTimespan, result.ToString());
        }
        #endregion
    }
}
