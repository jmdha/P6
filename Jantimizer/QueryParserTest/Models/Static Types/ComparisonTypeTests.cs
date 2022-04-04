using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserTest.Models.Static_Types
{
    [TestClass]
    public class ComparisonTypeTests
    {
        #region GetOperatorType

        [TestMethod]
        [DataRow("=", ComparisonType.Type.Equal)]
        [DataRow("<", ComparisonType.Type.Less)]
        [DataRow(">", ComparisonType.Type.More)]
        [DataRow("<=", ComparisonType.Type.EqualOrLess)]
        [DataRow(">=", ComparisonType.Type.EqualOrMore)]
        public void Can_GetOperatorType(string text, ComparisonType.Type expectedType)
        {
            // ARRANGE
            // ACT
            var retType = ComparisonType.GetOperatorType(text);

            // ASSERT
            Assert.AreEqual(expectedType, retType);
        }

        [TestMethod]
        [DataRow("equal")]
        [DataRow("non existent")]
        [DataRow(" abc = <=")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Cant_GetOperatorType_IfNotExist(string text)
        {
            // ARRANGE
            // ACT
            ComparisonType.GetOperatorType(text);
        }

        #endregion

        #region GetOperatorString

        [TestMethod]
        [DataRow("=", ComparisonType.Type.Equal)]
        [DataRow("<", ComparisonType.Type.Less)]
        [DataRow(">", ComparisonType.Type.More)]
        [DataRow("<=", ComparisonType.Type.EqualOrLess)]
        [DataRow(">=", ComparisonType.Type.EqualOrMore)]
        public void Can_GetOperatorString(string expText, ComparisonType.Type type)
        {
            // ARRANGE
            // ACT
            var retText = ComparisonType.GetOperatorString(type);

            // ASSERT
            Assert.AreEqual(expText, retText);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Cant_GetOperatorString_IfNone()
        {
            // ARRANGE
            // ACT
            ComparisonType.GetOperatorString(ComparisonType.Type.None);
        }

        #endregion
    }
}
