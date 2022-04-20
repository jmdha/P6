using DatabaseConnector.Connectors;
using QueryParser.Exceptions;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tools.Models;

[assembly: InternalsVisibleTo("QueryParserTest")]

namespace QueryParser.QueryParsers
{
    public class PostgresParser : IQueryParser
    {
        private ConnectionProperties? ConnectorProperties { get; set; }

        public PostgresParser(ConnectionProperties? connectorProperties = null)
        {
            ConnectorProperties = connectorProperties;
        }

        public bool DoesQueryMatch(string query)
        {
            if (ConnectorProperties == null)
                throw new ArgumentNullException("Connector was not set for the query parser!");
            try
            {
                using (var connector = new PostgreSqlConnector(ConnectorProperties))
                    connector.ExplainQuery(query);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DoesQueryMatchAsync(string query)
        {
            if (ConnectorProperties == null)
                throw new ArgumentNullException("Connector was not set for the query parser!");
            try
            {
                using (var connector = new PostgreSqlConnector(ConnectorProperties))
                    await connector.ExplainQueryAsync(query);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ParserResult ParseQuery(string query)
        {
            var parsed = GetParserResult(query);
            parsed.Wait();
            return parsed.Result;
        }

        public async Task<ParserResult> ParseQueryAsync(string query)
        {
            var parsed = await GetParserResult(query);
            return parsed;
        }

        private async Task<ParserResult> GetParserResult(string query)
        {
            string explanationTextBlock = await GetPGExplainationTextBlockAsync(query);

            return AnalyseExplanationText(explanationTextBlock);
        }

        internal ParserResult AnalyseExplanationText(string explanationText)
        {
            var result = new ParserResult();

            InsertTables(explanationText, ref result);
            InsertFilters(explanationText, ref result);
            InsertJoins(explanationText, ref result);
            InsertConditions(explanationText, ref result);

            return result;
        }


        internal static Regex TableFinder = new Regex(@"->.*?\sScan(?:\susing \w+)?\son\s(?<tableName>\w+)(?:\s(?<alias>\w+))?  \(cost=", RegexOptions.Compiled);
        internal void InsertTables(string queryExplanationTextBlock, ref ParserResult result)
        {
            MatchCollection matches = TableFinder.Matches(queryExplanationTextBlock);

            int id = 0;
            foreach (Match match in matches)
            {
                string alias = GetAliasFromRegexMatch(match);

                result.Tables[alias] = new TableReferenceNode(
                    id: id++,
                    tableName: match.Groups["tableName"].Value,
                    alias: alias
                );
            }
        }

        internal string GetAliasFromRegexMatch(Match match)
        {
            if (match.Groups["alias"].Success)
                return match.Groups["alias"].Value;

            return match.Groups["tableName"].Value;
        }

        internal static Regex JoinFinder = new Regex(@"(Join Filter|Hash Cond|Merge Cond): +(?<predicates>.+)?", RegexOptions.Compiled);
        internal void InsertJoins(string queryExplanationTextBlock, ref ParserResult result)
        {
            MatchCollection matches = JoinFinder.Matches(queryExplanationTextBlock);

            int id = 0;
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;

                var predicate = groups["predicates"].Value;

                var join = new JoinNode(
                    id++,
                    predicate,
                    ExtrapolateRelation(predicate, result)
                );

                result.Joins.Add(join);
            }
        }


        internal static readonly Regex FilterAndConditionFinder = new Regex(@"
                (?:^\s*->.*?\sScan(?:\susing\s\w+)?\son\s(?<tableName>\w+)(?:\s(?<alias>\w+))?\s\s\(cost=[^\n]+)

                (?:\s+ Index\ Cond:\ \((?<joinProp>\w+)\ (?<relation>[=<>]{1,2})\ (?<otherAlias>\w+)\.(?<otherProp>\w+)\))?

                (?:\s+Filter:\ \((?<filterProp>\w+)\ (?<filterCondition>[=<>]{1,2})\ (?<filterValue>\d+)\))?
            ",
            RegexOptions.Compiled |
            RegexOptions.Multiline |
            RegexOptions.IgnorePatternWhitespace
        );


        internal void InsertFilters(string queryExplanationTextBlock, ref ParserResult result)
        {
            var matches = FilterAndConditionFinder.Matches(queryExplanationTextBlock);

            int id = 0;
            foreach (Match match in matches)
            {
                if (!match.Groups["filterProp"].Success)
                    continue;
                
                var tableRef = result.Tables[GetAliasFromRegexMatch(match)];

                result.Filters.Add(new FilterNode(
                    id: id,
                    tableReference: tableRef,
                    attributeName: match.Groups["filterProp"].Value,
                    comType: ComparisonType.GetOperatorType(match.Groups["filterCondition"].Value),
                    constant: match.Groups["filterValue"].Value
                ));
                id++;
            }
        }

        internal void InsertConditions(string queryExplanationTextBlock, ref ParserResult result)
        {
            var matches = FilterAndConditionFinder.Matches(queryExplanationTextBlock);

            int id = 0;
            foreach (Match match in matches)
            {
                if (!match.Groups["joinProp"].Success)
                    continue;

                GroupCollection groups = match.Groups;
                var tableRef1 = result.GetTableRef(GetAliasFromRegexMatch(match));
                var tableRef2 = result.GetTableRef(groups["otherAlias"].Value);

                string predicate = $"{tableRef1.Alias}.{groups["joinProp"]} {groups["relation"]} {tableRef2.Alias}.{groups["otherProp"]}";

                var join = new JoinNode(
                    id++,
                    predicate,
                    ExtrapolateRelation(predicate, result)
                );

                result.Joins.Add(join);
            }
        }

        internal async Task<string> GetPGExplainationTextBlockAsync(string query)
        {
            if (ConnectorProperties == null)
                throw new ArgumentNullException("Connector was not set for the query parser!");
            var explanation = new DataSet();
            using (var connector = new PostgreSqlConnector(ConnectorProperties))
                explanation = await connector.ExplainQueryAsync(query);
            var rawRows = explanation.Tables[0].Rows;

            var stringRows = new List<string>();

            foreach (DataRow row in rawRows)
            {
                object queryPlan = row["QUERY PLAN"];
                if (queryPlan == null)
                    throw new NullReferenceException($"\"QUERY PLAN\" not found from running '{query}' on postgres. Verify the connection");

                string? rowStr = queryPlan.ToString();

                if (!string.IsNullOrEmpty(rowStr))
                    stringRows.Add(rowStr);
            }

            return string.Join('\n', stringRows);
        }

        internal JoinPredicateRelation ExtrapolateRelation(string predicate, ParserResult result)
        {
            RelationType.Type[] relationTypes = new RelationType.Type[] { RelationType.Type.And, RelationType.Type.Or };
            string[] sides = new string[] {};
            RelationType.Type relationType = RelationType.Type.None;
            for (int i = 0; i < relationTypes.Length; i++)
            {
                sides = predicate.Split(RelationType.GetRelationString(relationTypes[i]));
                if (sides.Length == 2)
                {
                    relationType = relationTypes[i];
                    break;
                }
            }
            if (relationType == RelationType.Type.None || sides.Length < 1)
                return new JoinPredicateRelation(ExtrapolateJoinPredicate(predicate.Replace("(", "").Replace(")", ""), result));
            else if (sides.Length != 2)
                throw new InvalidPredicateException("Somehow only had one side!", predicate);

            JoinPredicateRelation leftRelation = ExtrapolateRelation(sides[0], result);
            JoinPredicateRelation rightRelation = ExtrapolateRelation(sides[1], result);

            return new JoinPredicateRelation(leftRelation, rightRelation, relationType);
        }

        internal JoinPredicate ExtrapolateJoinPredicate(string predicate, ParserResult result)
        {
            var operatorTypes = (ComparisonType.Type[])Enum.GetValues(typeof(ComparisonType.Type));
            string[] predicateSplit = new string[] {};
            ComparisonType.Type comparisonType = ComparisonType.Type.None;
            foreach (var op in operatorTypes)
            {
                if (op == ComparisonType.Type.None)
                    continue;
                string operatorString = ComparisonType.GetOperatorString(op);
                if (predicate.Contains(operatorString))
                {
                    predicateSplit = predicate.Split($" {operatorString} ");
                    comparisonType = op;
                    break;
                }
            }
            if (comparisonType == ComparisonType.Type.None)
                throw new InvalidOperatorException("Has no operator!", predicate);

            string[] leftSplit = predicateSplit[0].Split(".");
            string[] rightSplit = predicateSplit[1].Split(".");

            if (leftSplit.Length != 2 || rightSplit.Length != 2)
                throw new InvalidPredicateException($"Invalid split in predicate '{predicateSplit[0]} {predicateSplit[1]}'", predicate);

            return new JoinPredicate(
                result.GetTableRef(leftSplit[0].Trim()),
                leftSplit[1].Trim(),
                result.GetTableRef(rightSplit[0].Trim()),
                rightSplit[1].Trim(),
                predicate.Trim(),
                comparisonType
                );
        }
    }
}
