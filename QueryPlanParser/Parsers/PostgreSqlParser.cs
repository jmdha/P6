using QueryPlanParser.Exceptions;
using QueryPlanParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryPlanParser.Parsers
{
    public class PostgreSqlParser : BasePlanParser
    {
        public override AnalysisResult ParsePlan(DataSet planData)
        {
            Queue<AnalysisResultWithIndent> resultQueue;

            string[] explainRows = GetExplainRows(planData.Tables[0].Rows);
            var resultRows = ParseQueryAnalysisRows(explainRows);
            resultQueue = new Queue<AnalysisResultWithIndent>(resultRows);

            if (resultQueue.Count == 0)
                throw new BadQueryPlanInputException("Analysis got no rows!");

            return RunAnalysis(resultQueue);
        }

        internal AnalysisResult RunAnalysis(Queue<AnalysisResultWithIndent> rows)
        {
            AnalysisResultWithIndent? row = rows.Dequeue();
            var analysisRes = row.AnalysisResult;

            // Recursively add subqueries
            while (rows.Count > 0)
            {
                if (rows.Peek().Indentation > row.Indentation)
                    analysisRes.SubQueries.Add(RunAnalysis(rows));
                else
                    break;
            }

            return analysisRes;
        }

        internal IEnumerable<AnalysisResultWithIndent> ParseQueryAnalysisRows(string[] rows)
        {
            foreach (string rowStr in rows)
            {
                var parsed = ParseQueryAnalysisRow(rowStr);
                if (parsed != null)
                    yield return parsed;
            }
        }

        internal string[] GetExplainRows(DataRowCollection rows)
        {
            if (rows.Count > 0)
            {
                List<string> explainRows = new List<string>();
                foreach (DataRow row in rows)
                {
                    if (!row.Table.Columns.Contains("QUERY PLAN"))
                        throw new BadQueryPlanInputException("Database did not return a correct query plan!");
                    explainRows.Add(row["QUERY PLAN"].ToString()!);
                }
                return explainRows.ToArray();
            }
            throw new BadQueryPlanInputException("Database did not return a correct query plan!");
        }

        private static Regex RowParserRegex = new Regex(@"^(?<indentation> *(?:->)? *)(?<name>[^(\n]+)\(cost=(?<costMin>\d+.\d+)\.\.(?<costMax>\d+.\d+) rows=(?<estimatedRows>\d+) width=(?<width>\d+)\) \(actual time=(?<timeMin>\d+.\d+)\.\.(?<timeMax>\d+.\d+) rows=(?<actualRows>\d+) loops=(?<loops>\d+)\)", RegexOptions.Compiled);

        /// <summary>
        /// Returns null if the row isn't a new subquery, e.g. if the row is the condition on a join.
        /// </summary>
        /// <param name="rowStr">The content from a single row after running EXPLAIN ANALYZE</param>
        /// <returns>AnalysisResult, IndentationSize</returns>
        internal AnalysisResultWithIndent? ParseQueryAnalysisRow(string rowStr)
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
                rowData["name"].Value.Trim(),
                decimal.Parse(rowData["costMin"].Value, System.Globalization.CultureInfo.InvariantCulture),
                ulong.Parse(rowData["estimatedRows"].Value, System.Globalization.CultureInfo.InvariantCulture),
                ulong.Parse(rowData["actualRows"].Value, System.Globalization.CultureInfo.InvariantCulture),
                TimeSpanFromMs(decimal.Parse(rowData["timeMax"].Value, System.Globalization.CultureInfo.InvariantCulture))
            );

            // Return
            return new AnalysisResultWithIndent(analysisResult, indentation);
        }
    }
}
