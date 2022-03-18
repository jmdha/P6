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

namespace QueryPlanParserTests.SystemTests.Parsers
{
    [TestClass]
    public class PostgreSqlParserSystemTests
    {
        #region ParsePlan

        [TestMethod]
        [DataRow(new string[] {
            "Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)",
            "  ->  Materialize  (cost=0.00..1.15 rows=10 width=8) (actual time=0.000..0.001 rows=10 loops=50)",
            "        ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8) (actual time=0.009..0.010 rows=10 loops=1)"
        }, new string[] {
            "Nested Loop",
            "Materialize",
            "Seq Scan on a"
        }, new int[]
        {
            0,
            1,
            2
        })]
        [DataRow(new string[] {
            "Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)",
            "  ->  Materialize  (cost=0.00..1.15 rows=10 width=8) (actual time=0.000..0.001 rows=10 loops=50)",
            "        ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8) (actual time=0.009..0.010 rows=10 loops=1)",
            "        ->  Seq Scan on b  (cost=0.00..1.10 rows=10 width=8) (actual time=0.009..0.010 rows=10 loops=1)"
        }, new string[] {
            "Nested Loop",
            "Materialize",
            "Seq Scan on a",
            "Seq Scan on b"
        }, new int[]
        {
            0,
            1,
            2,
            2
        })]
        [DataRow(new string[] {
            "Nested Loop  (cost=0.00..10.13 rows=167 width=16) (actual time=0.028..0.072 rows=45 loops=1)",
            "  ->  Materialize  (cost=0.00..1.15 rows=10 width=8) (actual time=0.000..0.001 rows=10 loops=50)",
            "        ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8) (actual time=0.009..0.010 rows=10 loops=1)",
            "        ->  Seq Scan on b  (cost=0.00..1.10 rows=10 width=8) (actual time=0.009..0.010 rows=10 loops=1)",
            "  ->  Materialize2  (cost=0.00..1.15 rows=10 width=8) (actual time=0.000..0.001 rows=10 loops=50)",
        }, new string[] {
            "Nested Loop",
            "Materialize",
            "Seq Scan on a",
            "Seq Scan on b",
            "Materialize"
        }, new int[]
        {
            0,
            1,
            2,
            2,
            1
        })]
        public void Can_ParsePlan_WithCorrectData(string[] lines, string[] eNameOrder, int[] nameDepth)
        {
            // Arrange
            IPlanParser parser = new PostgreSqlParser();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("QUERY PLAN"));
            foreach (string line in lines)
                table.Rows.Add(AddRow(table, new string[] { line }));
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(table);

            // Act
            var result = parser.ParsePlan(dataSet);

            // Assert
            for (int i = 0; i < nameDepth.Length; i++)
                Assert.AreEqual(nameDepth[i], GetNameDepth(result, 0, eNameOrder[i]));
        }

        [TestMethod()]
        [ExpectedException(typeof(BadQueryPlanInputException))]
        public void Cant_ParsePlan_IfNoDataIsGiven()
        {
            // Arrange
            IPlanParser parser = new PostgreSqlParser();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("QUERY PLAN"));
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(table);

            // Act
            parser.ParsePlan(dataSet);
        }

        #endregion

        #region Private Test Methods

        private int? GetNameDepth(AnalysisResult res, int curDepth, string targetName)
        {
            if (res.Name == targetName)
            {
                return curDepth;
            }
            curDepth++;
            foreach (AnalysisResult innerRes in res.SubQueries)
            {
                var ret = GetNameDepth(innerRes, curDepth, targetName);
                if (ret != null)
                    return ret;
            }
            return null;
        }

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
