using Microsoft.VisualStudio.TestTools.UnitTesting;

using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;

namespace QueryParsers
{
    [TestClass]
    public class ParserResultTest
    {
        [TestMethod]
        [DataRow(0, "a", "a")]
        [DataRow(2, "a", "b")]
        [DataRow(3, "b", "a")]
        [DataRow(4, "b", "b")]
        public void GetTableRefTest(int id, string name, string alias) {
            ParserResult result = new ParserResult();
            result.Tables.Add(alias, new TableReferenceNode(id, name, alias));
            TableReferenceNode node = result.GetTableRef(alias);
            Assert.AreEqual(id, node.Id);
            Assert.AreEqual(name, node.TableName);
            Assert.AreEqual(alias, node.Alias);
        }
    }
}