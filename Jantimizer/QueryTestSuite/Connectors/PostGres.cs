using Npgsql;
using QueryTestSuite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostgresTestSuite.Connectors
{
    internal class PostGres : DbConnector
    {
        string ConnectionString { get; set; }
        public PostGres(string connectionString) : base("Postgress")
        {
            ConnectionString = connectionString;
        }


        public async Task<DataSet> CallQuery(string query)
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand(query, conn))
            {
                DataSet dt = new DataSet();
                using (NpgsqlDataAdapter sqlAdapter = new NpgsqlDataAdapter(cmd))
                {
                    await Task.Run(() => sqlAdapter.Fill(dt));
                }
                return dt;
            }
        }

        public string GetCardinalityEstimate(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetCardinalityActual(string query)
        {
            var result = await CallQuery(query);

            return result.Tables[0].Rows.Count;
        }

        public async Task<AnalysisResult> GetAnalysis(string query)
        {
            string analysisQuery = $"EXPLAIN ANALYSE {query}";
            DataSet analysis = await CallQuery(analysisQuery);


            Queue<Tuple<AnalysisResult, int>> results;

            try
            {
                results = new Queue<Tuple<AnalysisResult, int>>(ParseQueryAnalysisRows(analysis.Tables[0].Rows));
            }
            catch (NoNullAllowedException)
            {
                throw new NoNullAllowedException($"Unexpected null-value from PostGre Analysis of {analysisQuery}");
            }


            return RunAnalysis(results);
        }

        private AnalysisResult RunAnalysis(Queue<Tuple<AnalysisResult, int>> rows)
        {
            Tuple<AnalysisResult, int>? row = rows.Dequeue();
            var analysisRes = row.Item1;
            var indent = row.Item2;

            bool isCheckingForSubQueries = true;
            while (isCheckingForSubQueries)
            {
                Tuple<AnalysisResult, int>? nextRow;
                if (!rows.TryPeek(out nextRow))
                    break;

                var nextIndent = nextRow.Item2;

                if(nextIndent > indent)
                    analysisRes.SubQueries.Add(RunAnalysis(rows));
                else
                    isCheckingForSubQueries = false;
            }

            return analysisRes;
        }

        private Queue<Tuple<AnalysisResult, int>> ParseQueryAnalysisRows(DataRowCollection rows)
        {
            Queue<Tuple<AnalysisResult, int>> outQueue = new Queue<Tuple<AnalysisResult, int>>();

            foreach (DataRow row in rows)
            {
                var rowStr = row["QUERY PLAN"].ToString();
                if (string.IsNullOrEmpty(rowStr))
                    throw new NoNullAllowedException($"Unexpected null-value: {{{rowStr}}}");

                var parsed = ParseQueryAnalysisRow(rowStr);
                if (parsed != null)
                    outQueue.Enqueue(parsed);
            }

            return outQueue;
        }


        /// <summary>
        /// Returns null if the row isn't a new subquery, e.g. if the row is the condition on a join.
        /// </summary>
        /// <param name="rowStr">The content from a single row after running EXPLAIN ANALYZE</param>
        /// <returns>AnalysisResult, IndentationSize</returns>
        private Tuple<AnalysisResult, int>? ParseQueryAnalysisRow(string rowStr)
        {
            Regex rowParser = new Regex(@"^(?<indentation> *(?:->)? *)(?<name>[^(\n]+)\(cost=(?<costMin>\d+.\d+)\.\.(?<costMax>\d+.\d+) rows=(?<estimatedRows>\d+) width=(?<width>\d+)\) \(actual time=(?<timeMin>\d+.\d+)\.\.(?<timeMax>\d+.\d+) rows=(?<actualRows>\d+) loops=(?<loops>\d+)\)");

            var match = rowParser.Match(rowStr);

            if (!match.Success)
                return null;

            var rowData = rowParser.Match(rowStr).Groups;

            int indentation = rowData["indentation"].ToString().Length;

            decimal miliseconds = decimal.Parse(rowData["timeMax"].ToString()); // Decimal, instead of double, because the tiny, somewhat important number, has already been written to ascii as decimal.
            long nanoSeconds = (long)Math.Round(miliseconds * 1000000);

            var analysisResult = new AnalysisResult(
                rowData["name"].ToString().Trim(),
                decimal.Parse(rowData["costMin"].ToString()),
                int.Parse(rowData["estimatedRows"].ToString()),
                int.Parse(rowData["actualRows"].ToString()),
                new TimeSpan(nanoSeconds)
            );

            return Tuple.Create(analysisResult, indentation);
        }


        public string GetQueryPlan(string query)
        {
            throw new NotImplementedException();
        }
    }
}
