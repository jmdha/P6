using DatabaseConnector.Connectors;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QueryParser.QueryParsers
{
    public class JoinOrderBenchmarkParser /*: IQueryParser*/
    {
        private PostgreSqlConnector Connector { get; set; }

        public JoinOrderBenchmarkParser(PostgreSqlConnector connector)
        {
            Connector = connector;
        }


        public bool DoesQueryMatch(string query)
        {
            return true;
        }



        public async Task<ParserResult> ParseQuery(string query)
        {
            string explanationTextBlock = await GetPGExplainationTextBlock(query);

            var result = new ParserResult();

            InsertTables(explanationTextBlock, ref result);
            InsertFilters(explanationTextBlock, ref result);
            InsertJoins(explanationTextBlock, ref result);
            InsertConditions(explanationTextBlock, ref result);

            return result;
        }


        private static Regex TableFinder = new Regex(@"->.*?\sScan(?:\susing \w+)?\son\s(?<tableName>\w+)(?:\s(?<alias>\w+))?  \(cost=", RegexOptions.Compiled);
        private void InsertTables(string queryExplanationTextBlock, ref ParserResult result)
        {
            MatchCollection matches = TableFinder.Matches(queryExplanationTextBlock);

            int id = 0;
            foreach (Match match in matches)
            {
                string alias = getAliasFromRegexMatch(match);

                result.Tables[alias] = new TableReferenceNode(
                    id: id++,
                    tableName: match.Groups["tableName"].Value,
                    alias: alias
                );
            }
        }

        private string getAliasFromRegexMatch(Match match)
        {
            if (match.Groups["alias"] != null)
                return match.Groups["alias"].Value;

            return match.Groups["tableName"].Value;
        }

        private static Regex JoinFinder = new Regex(@": \((?<t1>\w+)\.(?<prop1>\w+) (?<relation>[=<>]{1,2}) (?<t2>\w+)\.(?<prop2>\w+)\)", RegexOptions.Compiled);
        private void InsertJoins(string queryExplanationTextBlock, ref ParserResult result)
        {
            MatchCollection matches = JoinFinder.Matches(queryExplanationTextBlock);

            int id = 0;
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                var tableRef1 = result.Tables[groups["t1"].Value];
                var tableRef2 = result.Tables[groups["t2"].Value];

                var join = new JoinNode(id++, tableRef1.Alias, tableRef2.Alias, $"{tableRef1.Alias}.{groups["prop1"]} {groups["relation"]} {tableRef2.Alias}.{groups["prop2"]}");

                tableRef1.Joins.Add(join);
                tableRef2.Joins.Add(join);

                result.Joins.Add(join);
            }
        }


        private static readonly Regex FilterAndConditionFinder = new Regex(@"
                (?:^\s*->.*?\sScan(?:\susing\s\w+)?\son\s(?<tableName>\w+)(?:\s(?<alias>\w+))?\s\s\(cost=[^\n]+)

                (?:\s+ Index\ Cond:\ \((?<joinProp>\w+)\ (?<relation>[=<>]{1,2})\ (?<otherAlias>\w+)\.(?<otherProp>\w+)\))?

                (?:\s+ Filter:\ \((?<filterProp>\w+)\ (?<filterCondition>[=<>]{1,2})\ (?<filterValue>\d+)\))?
            ",
            RegexOptions.Compiled |
            RegexOptions.Multiline |
            RegexOptions.IgnorePatternWhitespace
        );


        private void InsertFilters(string queryExplanationTextBlock, ref ParserResult result)
        {
            var matches = FilterAndConditionFinder.Matches(queryExplanationTextBlock);

            foreach (Match match in matches)
            {
                if (!match.Groups["filterProp"].Success)
                    continue;

                var tableRef = result.Tables[getAliasFromRegexMatch(match)];

                tableRef.Filters.Add(new FilterNode(
                    tableReference: tableRef,
                    attributeName: match.Groups["filterProp"].Value,
                    relation: match.Groups["filterCondition"].Value,
                    constant: match.Groups["filterValue"].Value
                ));
            }
        }

        private void InsertConditions(string queryExplanationTextBlock, ref ParserResult result)
        {
            var matches = FilterAndConditionFinder.Matches(queryExplanationTextBlock);

            int id = 0;
            foreach (Match match in matches)
            {
                if (!match.Groups["joinProp"].Success)
                    continue;

                GroupCollection groups = match.Groups;
                var tableRef1 = result.GetTableRef(getAliasFromRegexMatch(match));
                var tableRef2 = result.GetTableRef(groups["otherAlias"].Value);

                var join = new JoinNode(id++, tableRef1.Alias, tableRef2.Alias, $"{tableRef1.Alias}.{groups["joinProp"]} {groups["relation"]} {tableRef2.Alias}.{groups["otherProp"]}");

                tableRef1.Joins.Add(join);
                tableRef2.Joins.Add(join);

                result.Joins.Add(join);
            }
        }

        private async Task<string> GetPGExplainationTextBlock(string query)
        {
            string explainQuery = $"EXPLAIN {query}";
            var explanation = await Connector.CallQuery(explainQuery);
            var rawRows = explanation.Tables[0].Rows;

            var stringRows = new List<string>();

            foreach (DataRow row in rawRows)
            {
                object queryPlan = row["QUERY PLAN"];
                if (queryPlan == null)
                    throw new NullReferenceException($"\"QUERY PLAN\" not found from running '{explainQuery}' on postgres. Verify the connection");

                string? rowStr = queryPlan.ToString();

                if (!string.IsNullOrEmpty(rowStr))
                    stringRows.Add(rowStr);
            }

            return string.Join('\n', stringRows);
        }
    }
}
