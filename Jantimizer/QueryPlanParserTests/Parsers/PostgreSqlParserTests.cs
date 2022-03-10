using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryPlanParser;
using QueryPlanParser.Exceptions;
using QueryPlanParser.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryPlanParserTests.Parsers
{
    [TestClass]
    public class PostgreSqlParserTests
    {
        [TestMethod]
        [DataRow("line1", " line2", "  line3")]
        [DataRow("", " aaabbbaa", "  baba")]
        [DataRow("-10", " 138", "  {abc}")]
        public void Can_GetExplainRows_WithCorrectData(string line1, string line2, string line3)
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("QUERY PLAN"));

            table.Rows.Add(AddRow(table, new string[] { line1 }));
            table.Rows.Add(AddRow(table, new string[] { line2 }));
            table.Rows.Add(AddRow(table, new string[] { line3 }));

            DataRowCollection data = table.Rows;

            // Act
            var result = parser.GetExplainRows(data);

            // Assert
            Assert.AreEqual(line1, result[0]);
            Assert.AreEqual(line2, result[1]);
            Assert.AreEqual(line3, result[2]);
        }

        [TestMethod]
        [DataRow("line1", " line2", "  line3")]
        [DataRow("", " aaabbbaa", "  baba")]
        [DataRow("-10", " 138", "  {abc}")]
        [ExpectedException(typeof(BadQueryPlanInputException))]
        public void Cant_GetExplainRows_WithIncorrectData(string line1, string line2, string line3)
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Other Column Name"));

            table.Rows.Add(AddRow(table, new string[] { line1 }));
            table.Rows.Add(AddRow(table, new string[] { line2 }));
            table.Rows.Add(AddRow(table, new string[] { line3 }));

            DataRowCollection data = table.Rows;

            // Act
            parser.GetExplainRows(data);
        }

        private DataRow AddRow(DataTable dt, string[] data)
        {
            DataRow row = dt.NewRow();
            for(int i = 0; i < data.Length; i++)
                row[i] = data[i];
            return row;
        }
    }
}
