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
    public class RelationTypeTests
    {
        #region GetOperatorType

        [TestMethod]
        [DataRow("AND", RelationType.Type.And)]
        [DataRow("OR", RelationType.Type.Or)]
        [DataRow("         AND", RelationType.Type.And)]
        [DataRow("or   ", RelationType.Type.Or)]
        [DataRow(" a.b ", RelationType.Type.Predicate)]
        [DataRow(" a.b  bb aND ", RelationType.Type.Predicate)]
        public void Can_GetOperatorType(string text, RelationType.Type expectedType)
        {
            // ARRANGE
            // ACT
            var retType = RelationType.GetRelationType(text);

            // ASSERT
            Assert.AreEqual(expectedType, retType);
        }

        #endregion

        #region GetRelationString

        [TestMethod]
        [DataRow("AND", RelationType.Type.And)]
        [DataRow("OR", RelationType.Type.Or)]
        public void Can_GetRelationString(string expText, RelationType.Type type)
        {
            // ARRANGE
            // ACT
            var retText = RelationType.GetRelationString(type);

            // ASSERT
            Assert.AreEqual(expText, retText);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Cant_GetRelationString_IfNone()
        {
            // ARRANGE
            // ACT
            RelationType.GetRelationString(RelationType.Type.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Cant_GetRelationString_Predicate()
        {
            // ARRANGE
            // ACT
            RelationType.GetRelationString(RelationType.Type.Predicate);
        }

        #endregion
    }
}
