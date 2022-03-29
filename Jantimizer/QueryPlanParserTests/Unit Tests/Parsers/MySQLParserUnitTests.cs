using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryPlanParser;
using QueryPlanParser.Exceptions;
using QueryPlanParser.Models;
using QueryPlanParser.Parsers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryPlanParserTests.UnitTests.Parsers
{
    [TestClass]
    public class MySQLParserUnitTests
    {
        #region GetExplainRows

        [TestMethod]
        [DataRow("line1", " line2", "  line3")]
        [DataRow("a ", " aaabbbaa", "  baba")]
        [DataRow("-10", " 138", "  {abc}")]
        public void Can_GetExplainRows_WithCorrectData(string line1, string line2, string line3)
        {
            // Arrange
            MySQLParser parser = new MySQLParser();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("EXPLAIN"));

            table.Rows.Add(AddRow(table, new string[] { $"{line1}\n{line2}\n{line3}\n" }));

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
        [DataRow("a ", " aaabbbaa", "  baba")]
        [DataRow("-10", " 138", "  {abc}")]
        [ExpectedException(typeof(BadQueryPlanInputException))]
        public void Cant_GetExplainRows_WithIncorrectData(string line1, string line2, string line3)
        {
            // Arrange
            MySQLParser parser = new MySQLParser();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Other Column Name"));

            table.Rows.Add(AddRow(table, new string[] { $"{line1}\n{line2}\n{line3}\n" }));

            DataRowCollection data = table.Rows;

            // Act
            parser.GetExplainRows(data);
        }

        #endregion

        #region ParseQueryAnalysisRow

        [TestMethod]
        [DataRow("-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)", "Nested loop inner join", 1.10d, (ulong)2, (ulong)3, "00:00:00.0001000")]
        [DataRow("    -> Table scan on jan_table  (cost=0.45 rows=2) (actual time=0.023..0.025 rows=2 loops=1)", "Table scan on jan_table", 0.45d, (ulong)2, (ulong)2, "00:00:00.0000250")]
        [DataRow("    -> Filter: (jan_table.Id <= jano_table.Id)  (cost=0.18 rows=1) (actual time=0.032..0.036 rows=2 loops=2)", "Filter:", 0.18d, (ulong)1, (ulong)2, "00:00:00.0000360")]
        public void Can_ParseQueryAnalysisRow_WithCorrectData(string line, string eName, double eCost, ulong eRows, ulong aRows, string aTime)
        {
            // Arrange
            MySQLParser parser = new MySQLParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(eName, result.AnalysisQueryTree.Name);

            Assert.IsNotNull(result.AnalysisQueryTree.EstimatedCost);
            /* Decimal cannot be used in DataRows */
            Assert.AreEqual((decimal)eCost, (decimal)result.AnalysisQueryTree.EstimatedCost);

            Assert.IsNotNull(result.AnalysisQueryTree.EstimatedCardinality);
            Assert.AreEqual(eRows, (ulong)result.AnalysisQueryTree.EstimatedCardinality);

            Assert.IsNotNull(result.AnalysisQueryTree.ActualCardinality);
            Assert.AreEqual(aRows, (ulong)result.AnalysisQueryTree.ActualCardinality);

            Assert.IsNotNull(result.AnalysisQueryTree.ActualTime);
            Assert.AreEqual(aTime, result.AnalysisQueryTree.ActualTime.ToString());
        }

        [TestMethod]
        [DataRow("  Join Filter: (a.s > b.s)")]
        [DataRow("Planning Time: 0.440 ms")]
        public void Can_ParseQueryAnalysisRow_IgnoreNonCostRows(string line)
        {
            // Arrange
            MySQLParser parser = new MySQLParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow("-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)", 3)]
        [DataRow("    -> Table scan on jan_table  (cost=0.45 rows=2) (actual time=0.023..0.025 rows=2 loops=1)", 7)]
        [DataRow("        -> Index range scan on jano_table (re-planned for each iteration)  (cost=0.18 rows=2) (actual time=0.032..0.034 rows=2 loops=2)", 11)]
        public void Can_ParseQueryAnalysisRow_ParseIndent(string line, int expectedIndent)
        {
            // Arrange
            MySQLParser parser = new MySQLParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedIndent, result.Indentation);
        }

        [TestMethod]
        [DataRow("-> Nested loop inner join  (cost=1.b10 rows=2) (actual time=0.059..0.10a0 rows=3 loops=1)")]
        [DataRow("    -> Table scan on jan_table  (cost=0.45 rows=2) (actual time=0.023..0.025 rows=a2 loops=1)")]
        [DataRow("        -> Index range scan on jano_table (re-planned for each iteration)  (cost=0.18 rows=b2) (actual time=0.032..0.034 rows=2 loops=2)")]
        public void Can_ParseQueryAnalysisRow_IgnoreIncorrectData(string line)
        {
            // Arrange
            MySQLParser parser = new MySQLParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            if (result?.AnalysisQueryTree.EstimatedCost != null)
                Assert.IsNull(result?.AnalysisQueryTree.ActualCardinality);

            if (result?.AnalysisQueryTree.ActualCardinality != null)
                Assert.IsNull(result?.AnalysisQueryTree.EstimatedCost);
        }

        #endregion

        #region ParseQueryAnalysisRows

        [TestMethod]
        [DataRow(new string[] { "badRow", "badRow", "badRow" }, 0)]
        [DataRow(new string[] { "-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)", "badRow", "badRow" }, 1)]
        [DataRow(new string[] { "-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)", "-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)", "badRow" }, 2)]
        [DataRow(new string[] { "badRow", "-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)", "-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)" }, 2)]
        public void Can_ParseQueryAnalysisRows(string[] lines, int eCount)
        {
            // Arrange
            MySQLParser parser = new MySQLParser();

            // Act
            var result = parser.ParseQueryAnalysisRows(lines);

            // Assert
            Assert.AreEqual(eCount, result.Count());
        }

        [TestMethod]
        public void Can_ParseQueryAnalysisRows_WithNoRows()
        {
            // Arrange
            MySQLParser parser = new MySQLParser();

            // Act
            var result = parser.ParseQueryAnalysisRows(new string[0]);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        #endregion

        #region RunAnalysis

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Cant_RunAnalysis_WithNoData()
        {
            // Arrange
            MySQLParser parser = new MySQLParser();

            // Act
            parser.RunAnalysis(new Queue<AnalysisResultWithIndent>());
        }

        [TestMethod]
        public void Can_RunAnalysis_WithOneRow()
        {
            // Arrange
            MySQLParser parser = new MySQLParser();
            List<AnalysisResultWithIndent> list = new List<AnalysisResultWithIndent>();
            AnalysisResultQueryTree eResult = new AnalysisResultQueryTree("name", 0, 0, 0, new TimeSpan());
            list.Add(new AnalysisResultWithIndent(eResult, 0));

            // Act
            var result = parser.RunAnalysis(new Queue<AnalysisResultWithIndent>(list));

            // Assert
            Assert.AreEqual(eResult, result);
        }

        [TestMethod]
        public void Can_RunAnalysis_WithMultipleRows1()
        {
            // Arrange
            MySQLParser parser = new MySQLParser();
            List<AnalysisResultWithIndent> list = new List<AnalysisResultWithIndent>();
            AnalysisResultQueryTree eResult1 = new AnalysisResultQueryTree("name", 0, 0, 0, new TimeSpan());
            AnalysisResultQueryTree eResult2 = new AnalysisResultQueryTree("name", 0, 0, 0, new TimeSpan());
            AnalysisResultQueryTree eResult3 = new AnalysisResultQueryTree("name", 0, 0, 0, new TimeSpan());

            list.Add(new AnalysisResultWithIndent(eResult1, 0));
            list.Add(new AnalysisResultWithIndent(eResult2, 6));
            list.Add(new AnalysisResultWithIndent(eResult3, 12));

            // Act
            var result = parser.RunAnalysis(new Queue<AnalysisResultWithIndent>(list));

            // Assert
            Assert.AreEqual(eResult1, result);
            Assert.AreEqual(eResult2, result.SubQueries[0]);
            Assert.AreEqual(eResult3, result.SubQueries[0].SubQueries[0]);
        }

        [TestMethod]
        public void Can_RunAnalysis_WithMultipleRows2()
        {
            // Arrange
            MySQLParser parser = new MySQLParser();
            List<AnalysisResultWithIndent> list = new List<AnalysisResultWithIndent>();
            AnalysisResultQueryTree eResult1 = new AnalysisResultQueryTree("name", 0, 0, 0, new TimeSpan());
            AnalysisResultQueryTree eResult2 = new AnalysisResultQueryTree("name", 0, 0, 0, new TimeSpan());
            AnalysisResultQueryTree eResult3 = new AnalysisResultQueryTree("name", 0, 0, 0, new TimeSpan());

            list.Add(new AnalysisResultWithIndent(eResult1, 0));
            list.Add(new AnalysisResultWithIndent(eResult2, 6));
            list.Add(new AnalysisResultWithIndent(eResult3, 6));

            // Act
            var result = parser.RunAnalysis(new Queue<AnalysisResultWithIndent>(list));

            // Assert
            Assert.AreEqual(eResult1, result);
            Assert.AreEqual(eResult2, result.SubQueries[0]);
            Assert.AreEqual(eResult3, result.SubQueries[1]);
        }

        #endregion

        #region Private Test Methods

        private DataRow AddRow(DataTable dt, string[] data)
        {
            DataRow row = dt.NewRow();
            for(int i = 0; i < data.Length; i++)
                row[i] = data[i];
            return row;
        }

        #endregion
    }
}
