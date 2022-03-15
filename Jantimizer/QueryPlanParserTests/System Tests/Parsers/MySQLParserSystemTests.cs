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
    public class MySQLParserSystemTests
    {
        #region ParsePlan

        [TestMethod]
        [DataRow(
            "-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)\n" +
            "    -> Table scan on jan_table  (cost=0.45 rows=2) (actual time=0.023..0.025 rows=2 loops=1)\n" +
            "    -> Filter: (jan_table.Id <= jano_table.Id)  (cost=0.18 rows=1) (actual time=0.032..0.036 rows=2 loops=2)\n"
        , new string[] {
            "Nested loop inner join",
            "Table scan on jan_table",
            "Filter:"
        }, new int[]
        {
            0,
            1,
            1
        })]
        [DataRow(
            "-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)\n" +
            "    -> Table scan on jan_table  (cost=0.45 rows=2) (actual time=0.023..0.025 rows=2 loops=1)\n" +
            "    -> Filter: (jan_table.Id <= jano_table.Id)  (cost=0.18 rows=1) (actual time=0.032..0.036 rows=2 loops=2)\n" +
            "        -> Index range scan on jano_table (re-planned for each iteration)  (cost=0.18 rows=2) (actual time=0.032..0.034 rows=2 loops=2)\n"
        , new string[] {
            "Nested loop inner join",
            "Table scan on jan_table",
            "Filter:",
            "Index range scan on jano_table"
        }, new int[]
        {
            0,
            1,
            1,
            2
        })]
        [DataRow(
            "-> Nested loop inner join  (cost=1.10 rows=2) (actual time=0.059..0.100 rows=3 loops=1)\n" +
            "    -> Table scan on jan_table  (cost=0.45 rows=2) (actual time=0.023..0.025 rows=2 loops=1)\n" +
            "    -> Filter: (jan_table.Id <= jano_table.Id)  (cost=0.18 rows=1) (actual time=0.032..0.036 rows=2 loops=2)\n" +
            "        -> Index range scan on jano_table (re-planned for each iteration)  (cost=0.18 rows=2) (actual time=0.032..0.034 rows=2 loops=2)\n" +
            "    -> Table scan on jar_table  (cost=0.45 rows=2) (actual time=0.023..0.025 rows=2 loops=1)\n"
        , new string[] {
            "Nested loop inner join",
            "Table scan on jan_table",
            "Filter:",
            "Index range scan on jano_table",
            "Table scan on jar_table"
        }, new int[]
        {
            0,
            1,
            1,
            2,
            1
        })]
        public void Can_ParsePlan_WithCorrectData(string lines, string[] eNameOrder, int[] nameDepth)
        {
            // Arrange
            IPlanParser parser = new MySQLParser();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("EXPLAIN"));
            table.Rows.Add(AddRow(table, new string[] { lines }));
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
            table.Columns.Add(new DataColumn("EXPLAIN"));
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
