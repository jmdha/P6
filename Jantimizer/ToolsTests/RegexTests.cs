using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using Tools.Regex;

namespace ToolsTests
{
    [TestClass]
    public class RegexTests
    {
        #region GetRegexVal

        [TestMethod]
        [DataRow("start65end", 65)]
        public void Can_GetRegexVal_Int(string str, int expected)
        {
            // Arrange
            Regex regex = new Regex("start(?<groupName>.+)end$");

            // Act
            var match = regex.Match(str);

            int actual = RegexHelperFunctions.GetRegexVal<int>(match, "groupName");

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(int));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("startJanend", "Jan")]
        [DataRow("start end", " ")]
        public void Can_GetRegexVal_String(string str, string expected)
        {
            // Arrange
            Regex regex = new Regex("start(?<groupName>.+)end$");

            // Act
            var match = regex.Match(str);

            string actual = RegexHelperFunctions.GetRegexVal<string>(match, "groupName");

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(string));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("startend")]
        [ExpectedException(typeof(ArgumentException))]
        public void Can_GetRegexVal_NonNullableNull(string str)
        {
            // Arrange
            Regex regex = new Regex("start(?<groupName>.+)?end$");

            // Act
            var match = regex.Match(str);

            int actual = RegexHelperFunctions.GetRegexVal<int>(match, "groupName");
        }
        
        [TestMethod]
        [DataRow("startend")]
        public void Can_GetRegexVal_Nullable(string str)
        {
            // Arrange
            Regex regex = new Regex("start(?<groupName>.+)?end$");

            // Act
            var match = regex.Match(str);

            int? actual = RegexHelperFunctions.GetRegexValNullable<int>(match, "groupName");

            // Assert
            Assert.IsNull(actual);
        }

        #endregion


    }
}