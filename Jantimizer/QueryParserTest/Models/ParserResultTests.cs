using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserTest.Models
{
    [TestClass]
    public class ParserResultTests
    {
        [TestMethod]
        [DataRow("A", "TableA")]
        [DataRow("B", "TableB")]
        [DataRow("C", "TableC")]
        [DataRow("D", "TableD")]
        public void Can_GetTableRef_IfThere(string alias, string expTableName)
        {
            // ARRANGE
            var res = new ExplainResult();
            res.Tables.Add("A", new TableReferenceNode(0, "TableA", "A"));
            res.Tables.Add("B", new TableReferenceNode(1, "TableB", "B"));
            res.Tables.Add("C", new TableReferenceNode(2, "TableC", "C"));
            res.Tables.Add("D", new TableReferenceNode(3, "TableD", "D"));

            // ACT
            var tableRef = res.GetTableRef(alias);

            // ASSERT
            Assert.AreEqual(expTableName, tableRef.TableName);
            Assert.AreEqual(alias, tableRef.Alias);
        }

        [TestMethod]
        [DataRow("E")]
        [DataRow("AAAABA")]
        [DataRow("ABCD")]
        [DataRow("1")]
        [ExpectedException(typeof(NullReferenceException))]
        public void Cant_GetTableRef_IfNotThere(string nonExistingAlias)
        {
            // ARRANGE
            var res = new ExplainResult();
            res.Tables.Add("A", new TableReferenceNode(0, "TableA", "A"));
            res.Tables.Add("B", new TableReferenceNode(1, "TableB", "B"));
            res.Tables.Add("C", new TableReferenceNode(2, "TableC", "C"));
            res.Tables.Add("D", new TableReferenceNode(3, "TableD", "D"));

            // ACT
            var tableRef = res.GetTableRef(nonExistingAlias);
        }
    }
}
