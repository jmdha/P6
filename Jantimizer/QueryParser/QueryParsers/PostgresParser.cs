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
    public class PostgresParser : IQueryParser
    {
        private PostgreSqlConnector Connector { get; set; }

        public PostgresParser(PostgreSqlConnector connector)
        {
            Connector = connector;
        }


        public bool DoesQueryMatch(string query)
        {
            return true;
        }


        public List<INode> ParseQuery(string query)
        {
            var parsed = ParseQueryAsync(query);
            parsed.Wait();
            return parsed.Result.Joins.Select(j => j as INode).ToList();
        }

        public async Task<ParserResult> ParseQueryAsync(string query)
        {
            string explanationTextBlock = await GetPGExplainationTextBlock(query);

            return AnalyseExplanationText(explanationTextBlock);
        }

        public static ParserResult AnalyseExplanationText(string explanationText)
        {
            var result = new ParserResult();

            InsertTables(explanationText, ref result);
            InsertFilters(explanationText, ref result);
            InsertJoins(explanationText, ref result);
            InsertConditions(explanationText, ref result);

            return result;
        }


        private static Regex TableFinder = new Regex(@"->.*?\sScan(?:\susing \w+)?\son\s(?<tableName>\w+)(?:\s(?<alias>\w+))?  \(cost=", RegexOptions.Compiled);
        private static void InsertTables(string queryExplanationTextBlock, ref ParserResult result)
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

        private static string GetAliasFromRegexMatch(Match match)
        {
            if (match.Groups["alias"].Success)
                return match.Groups["alias"].Value;

            return match.Groups["tableName"].Value;
        }

        private static Regex JoinFinder = new Regex(@"Join Filter: +(?<predicates>.+)?", RegexOptions.Compiled);
        private static Regex TableWithinJoinFinder = new Regex(@"(?<tableNames>[a-z])[.]", RegexOptions.Compiled);
        private static void InsertJoins(string queryExplanationTextBlock, ref ParserResult result)
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


        private static readonly Regex FilterAndConditionFinder = new Regex(@"
                (?:^\s*->.*?\sScan(?:\susing\s\w+)?\son\s(?<tableName>\w+)(?:\s(?<alias>\w+))?\s\s\(cost=[^\n]+)

                (?:\s+ Index\ Cond:\ \((?<joinProp>\w+)\ (?<relation>[=<>]{1,2})\ (?<otherAlias>\w+)\.(?<otherProp>\w+)\))?

                (?:\s+ Filter:\ \((?<filterProp>\w+)\ (?<filterCondition>[=<>]{1,2})\ (?<filterValue>\d+)\))?
            ",
            RegexOptions.Compiled |
            RegexOptions.Multiline |
            RegexOptions.IgnorePatternWhitespace
        );


        private static void InsertFilters(string queryExplanationTextBlock, ref ParserResult result)
        {
            var matches = FilterAndConditionFinder.Matches(queryExplanationTextBlock);

            foreach (Match match in matches)
            {
                if (!match.Groups["filterProp"].Success)
                    continue;

                var tableRef = result.Tables[GetAliasFromRegexMatch(match)];

                tableRef.Filters.Add(new FilterNode(
                    tableReference: tableRef,
                    attributeName: match.Groups["filterProp"].Value,
                    relation: match.Groups["filterCondition"].Value,
                    constant: match.Groups["filterValue"].Value
                ));
            }
        }

        private static void InsertConditions(string queryExplanationTextBlock, ref ParserResult result)
        {
            var matches = FilterAndConditionFinder.Matches(queryExplanationTextBlock);

            int id = 0;
            foreach (Match match in matches)
            {
                if (!match.Groups["joinProp"].Success)
                    continue;

                GroupCollection groups = match.Groups;

                var join = new JoinNode(
                    id++,
                    (string) null,
                    null
                );

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

        public static JoinPredicateRelation ExtrapolateRelation(string predicate, ParserResult result)
        {
            JoinPredicateRelation.RelationType[] relationTypes = new JoinPredicateRelation.RelationType[] { JoinPredicateRelation.RelationType.And, JoinPredicateRelation.RelationType.Or };
            string[] sides = new string[] {};
            JoinPredicateRelation.RelationType relationType = JoinPredicateRelation.RelationType.None;
            for (int i = 0; i < relationTypes.Length; i++)
            {
                sides = predicate.Split(JoinPredicateRelation.GetRelationString(relationTypes[i]));
                if (sides.Length == 2)
                {
                    relationType = relationTypes[i];
                    break;
                }
            }
            if (relationType == JoinPredicateRelation.RelationType.None || sides.Length < 1)
                return new JoinPredicateRelation(ExtrapolateJoinPredicate(predicate.Replace("(", "").Replace(")", ""), result));
            else if (sides.Length != 2)
                throw new InvalidDataException("Somehow only had one side " + predicate);

            JoinPredicateRelation leftRelation = ExtrapolateRelation(sides[0], result);
            JoinPredicateRelation rightRelation = ExtrapolateRelation(sides[1], result);

            return new JoinPredicateRelation(leftRelation, rightRelation, relationType);
        }

        public static JoinNode.JoinPredicate ExtrapolateJoinPredicate(string predicate, ParserResult result)
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
                throw new InvalidDataException("Has no operator " + predicate);

            string[] leftSplit = predicateSplit[0].Split(".");
            string[] rightSplit = predicateSplit[1].Split(".");

            if (leftSplit.Length != 2 || rightSplit.Length != 2)
                throw new InvalidDataException("Invalid split " + predicateSplit[0] + " " + predicateSplit[1]);

            return new JoinNode.JoinPredicate(
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
