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
    public class PostgreSqlParserUnitTests
    {
        #region GetExplainRows

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

        #endregion

        #region ParseQueryAnalysisRow

        [TestMethod]
        [DataRow("Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)", "Nested Loop", "0.00", (ulong)167, (ulong)45, "00:00:00.0000720")]
        [DataRow("  ->  Materialize  (cost=0.00..1.15 rows=10 width=8) (actual time=0.000..0.001 rows=10 loops=50)", "Materialize", "0.00", (ulong)10, (ulong)10, "00:00:00.0000010")]
        [DataRow("        ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8) (actual time=0.009..0.010 rows=10 loops=1)", "Seq Scan on a", "0.00", (ulong)10, (ulong)10, "00:00:00.0000100")]
        public void Can_ParseQueryAnalysisRow_WithCorrectData(string line, string eName, string eCost, ulong eRows, ulong aRows, string aTime)
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            Assert.AreEqual(eName, result.AnalysisResult.Name);
            Assert.AreEqual(eCost, result.AnalysisResult.EstimatedCost.ToString(System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(eRows, result.AnalysisResult.EstimatedCardinality);
            Assert.AreEqual(aRows, result.AnalysisResult.ActualCardinality);
            Assert.AreEqual(aTime, result.AnalysisResult.ActualTime.ToString());
        }

        [TestMethod]
        [DataRow("  Join Filter: (a.s > b.s)")]
        [DataRow("Planning Time: 0.440 ms")]
        public void Can_ParseQueryAnalysisRow_IgnoreNonCostRows(string line)
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow("Nested Loop  (cost=0.00..10.13 rows=167 width=16)")]
        [DataRow("  ->  Materialize  (cost=0.00..1.15 rows=10 width=8)")]
        [DataRow("        ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)")]
        public void Can_ParseQueryAnalysisRow_IgnoreRowsWithNoActuals(string line)
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow("Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)", 0)]
        [DataRow("  ->  Materialize  (cost=0.00..1.15 rows=10 width=8) (actual time=0.000..0.001 rows=10 loops=50)", 6)]
        [DataRow("        ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8) (actual time=0.009..0.010 rows=10 loops=1)", 12)]
        public void Can_ParseQueryAnalysisRow_ParseIndent(string line, int expectedIndent)
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            Assert.AreEqual(expectedIndent, result.Indentation);
        }

        [TestMethod]
        [DataRow("Nested Loop  (cost=0.00..10.13 rows=16a7 width=16) (actual time=0.028..0.072 rows=45b loops=1)")]
        [DataRow("  ->  Materialize  (cost=0.00..1.a15 rows=10 width=8) (actual time=0.24h000..0.001 rows=10 loops=a50)")]
        [DataRow("        ->  Seq Scan on a  (cost=0.0l0..1.10 rows=10 width=14g8) (actual time=0.009..0.010 rows=10 loops=1b)")]
        public void Can_ParseQueryAnalysisRow_IgnoreIncorrectData(string line)
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();

            // Act
            var result = parser.ParseQueryAnalysisRow(line);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region ParseQueryAnalysisRows

        [TestMethod]
        [DataRow(new string[] { "badRow", "badRow", "badRow" }, 0)]
        [DataRow(new string[] { "Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)", "badRow", "badRow" }, 1)]
        [DataRow(new string[] { "Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)", "Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)", "badRow" }, 2)]
        [DataRow(new string[] { "badRow", "Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)", "Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)" }, 2)]
        public void Can_ParseQueryAnalysisRows(string[] lines, int eCount)
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();

            // Act
            var result = parser.ParseQueryAnalysisRows(lines);

            // Assert
            Assert.AreEqual(eCount, result.Count());
        }

        [TestMethod]
        public void Can_ParseQueryAnalysisRows_WithNoRows()
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();

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
            PostgreSqlParser parser = new PostgreSqlParser();

            // Act
            parser.RunAnalysis(new Queue<AnalysisResultWithIndent>());
        }

        [TestMethod]
        public void Can_RunAnalysis_WithOneRow()
        {
            // Arrange
            PostgreSqlParser parser = new PostgreSqlParser();
            List<AnalysisResultWithIndent> list = new List<AnalysisResultWithIndent>();
            AnalysisResult eResult = new AnalysisResult("name", 0, 0, 0, new TimeSpan());
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
            PostgreSqlParser parser = new PostgreSqlParser();
            List<AnalysisResultWithIndent> list = new List<AnalysisResultWithIndent>();
            AnalysisResult eResult1 = new AnalysisResult("name", 0, 0, 0, new TimeSpan());
            AnalysisResult eResult2 = new AnalysisResult("name", 0, 0, 0, new TimeSpan());
            AnalysisResult eResult3 = new AnalysisResult("name", 0, 0, 0, new TimeSpan());

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
            PostgreSqlParser parser = new PostgreSqlParser();
            List<AnalysisResultWithIndent> list = new List<AnalysisResultWithIndent>();
            AnalysisResult eResult1 = new AnalysisResult("name", 0, 0, 0, new TimeSpan());
            AnalysisResult eResult2 = new AnalysisResult("name", 0, 0, 0, new TimeSpan());
            AnalysisResult eResult3 = new AnalysisResult("name", 0, 0, 0, new TimeSpan());

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
            for (int i = 0; i < data.Length; i++)
                row[i] = data[i];
            return row;
        }

        #endregion
    }
}
