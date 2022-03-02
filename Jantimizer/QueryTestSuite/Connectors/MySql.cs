using MySqlConnector;
using Npgsql;
using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryTestSuite.Connectors
{
    internal class MySql : DbConnector
    {
        public MySql(string connectionString) : base(nameof(MySql), connectionString)
        {

        }

        public override async Task<DataSet> CallQuery(string query)
        {
            await using var conn = new MySqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using (var cmd = new MySqlCommand(query, conn))
            using (var sqlAdapter = new MySqlDataAdapter(cmd))
            {
                DataSet dt = new DataSet();
                await Task.Run(() => sqlAdapter.Fill(dt));

                return dt;
            }
        }

        public async override Task<AnalysisResult> GetAnalysis(string query)
        {
            string analysisQuery = $"EXPLAIN ANALYZE {query}";
            DataSet analysis = await CallQuery(analysisQuery);

            Queue<AnalysisResultWithIndent> resultQueue;

            try
            {
                var resultRows = ParseQueryAnalysisRows(analysis.Tables[0].Rows);
                resultQueue = new Queue<AnalysisResultWithIndent>(resultRows);
            }
            catch (NoNullAllowedException)
            {
                throw new NoNullAllowedException($"Unexpected null-value from MySql Analysis of {analysisQuery}");
            }

            return RunAnalysis(resultQueue);
        }

        private AnalysisResult RunAnalysis(Queue<AnalysisResultWithIndent> rows)
        {
            AnalysisResultWithIndent? row = rows.Dequeue();
            var analysisRes = row.AnalysisResult;

            // Recursively add subqueries
            while (rows.Count > 0)
            {
                if (rows.Peek().indentation > row.indentation)
                    analysisRes.SubQueries.Add(RunAnalysis(rows));
                else
                    break;
            }

            return analysisRes;
        }

        private IEnumerable<AnalysisResultWithIndent> ParseQueryAnalysisRows(DataRowCollection rows)
        {
            var rowLineSplit = rows[0]["EXPLAIN"].ToString()!.Trim().Split('\n');
            foreach (string rowStr in rowLineSplit)
            {
                if (string.IsNullOrEmpty(rowStr))
                    throw new NoNullAllowedException($"Unexpected null-value: {{{rowStr}}}");

                var parsed = ParseQueryAnalysisRow(rowStr);
                if (parsed != null)
                    yield return parsed;
            }
        }



        private static Regex RowParserRegex = new Regex(@"^(?<indentation> *(?:->)? *)(?<name>[^(\n]+)(?:\((?<predicate>[^)]+)?\) *)?\(cost=(?<cost>\d+.\d+) rows=(?<estimatedRows>\d+)\) \(actual time=(?<timeMin>\d+.\d+)\.\.(?<timeMax>\d+.\d+) rows=(?<actualRows>\d+) loops=(?<loops>\d+)\)", RegexOptions.Compiled);

        /// <summary>
        /// Returns null if the row isn't a new subquery, e.g. if the row is the condition on a join.
        /// </summary>
        /// <param name="rowStr">The content from a single row after running EXPLAIN ANALYZE</param>
        /// <returns>AnalysisResult, IndentationSize</returns>
        private AnalysisResultWithIndent? ParseQueryAnalysisRow(string rowStr)
        {
            // Regex Matching
            var match = RowParserRegex.Match(rowStr);

            if (!match.Success)
                return null;

            var rowData = match.Groups;

            // Data Parsing
            int indentation = rowData["indentation"].ToString().Length;

            // Create Result
            var analysisResult = new AnalysisResult(
                rowData["name"].ToString().Trim(),
                decimal.Parse(rowData["cost"].Value),
                ulong.Parse(rowData["estimatedRows"].Value),
                ulong.Parse(rowData["actualRows"].Value),
                TimeSpanFromMs(decimal.Parse(rowData["timeMax"].Value))
            );

            // Return
            return new AnalysisResultWithIndent(analysisResult, indentation);
        }

        private class AnalysisResultWithIndent
        {
            public AnalysisResult AnalysisResult { get; set; }
            public int indentation { get; set; }

            public AnalysisResultWithIndent(AnalysisResult analysisResult, int indentation)
            {
                AnalysisResult = analysisResult;
                this.indentation = indentation;
            }
        }
    }

}
