﻿using QueryPlanParser.Exceptions;
using QueryPlanParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tools.Regex;

namespace QueryPlanParser.Parsers
{
    public class PostgreSqlParser : BasePlanParser
    {
        public override AnalysisResult ParsePlan(DataSet planData)
        {
            try
            {
                Queue<AnalysisResultWithIndent> resultQueue;

                string[] explainRows = GetExplainRows(planData.Tables[0].Rows);
                var resultRows = ParseQueryAnalysisRows(explainRows);
                resultQueue = new Queue<AnalysisResultWithIndent>(resultRows);

                if (resultQueue.Count == 0)
                    throw new BadQueryPlanInputException("Analysis got no rows!");

                return new AnalysisResult(RunAnalysis(resultQueue), planData, this.GetType().Name);
            }
            catch (Exception ex)
            {
                throw new QueryPlanParserErrorLogException(ex, planData);
            }
        }

        internal AnalysisResultQueryTree RunAnalysis(Queue<AnalysisResultWithIndent> rows)
        {
            AnalysisResultWithIndent? row = rows.Dequeue();
            var analysisRes = row.AnalysisQueryTree;

            // Recursively add subqueries
            while (rows.Count > 0)
            {
                if (rows.Peek().Indentation > row.Indentation)
                    analysisRes.SubQueries.Add(RunAnalysis(rows));
                else
                    break;
            }

            if (analysisRes.ActualCardinality == null)
                analysisRes.ActualCardinality = 0;
            if (analysisRes.ActualTime == null)
                analysisRes.ActualTime = new TimeSpan();
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

        private static Regex RowParserRegex = new Regex(@"^(?<indentation> *(?:->)? *)(?<name>[^(\n]+)\(cost=(?<costMin>\d+.\d+)\.\.(?<costMax>\d+.\d+) rows=(?<estimatedRows>\d+) width=(?<width>\d+)\)(?: \(actual time=(?<timeMin>\d+.\d+)\.\.(?<timeMax>\d+.\d+) rows=(?<actualRows>\d+) loops=(?<loops>\d+)\))?", RegexOptions.Compiled);

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

            decimal? timeMax = RegexHelperFunctions.GetRegexValNullable<decimal>(match, "timeMax");
            TimeSpan? timeCost = null;
            if (timeMax != null)
                timeCost = TimeSpanFromMs((decimal)timeMax);

            // Create Result
            var analysisResult = new AnalysisResultQueryTree(
                RegexHelperFunctions.GetRegexVal<string>(match, "name").Trim(),
                RegexHelperFunctions.GetRegexValNullable<decimal>(match, "costMin"),
                RegexHelperFunctions.GetRegexValNullable<ulong>(match, "estimatedRows"),
                RegexHelperFunctions.GetRegexValNullable<ulong>(match, "actualRows"),
                timeCost
            );

            // Return
            return new AnalysisResultWithIndent(analysisResult, indentation);
        }
    }
}
