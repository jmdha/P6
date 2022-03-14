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
    public class JoinOrderBenchmarkParser : IQueryParser
    {
        private PostgreSqlConnector Connector { get; set; }

        public JoinOrderBenchmarkParser(PostgreSqlConnector connector)
        {
            Connector = connector;
        }


        public bool DoesQueryMatch(string query)
        {

            var explanation = Connector.CallQuery($"EXPLAIN {query}");
            throw new NotImplementedException();
        }


        static Regex SelectFromWhereRegex = new Regex(@"
                ^\s*SELECT\s+
                    (?<select>.*?)\s+ # Captures everything between the first 'SELECT ' and the first ' FROM '
                FROM\s+
                    (?<from>.+)\s+    # Captures as much as possible, until the last occurance of 'WHERE'
                WHERE                 
                    \s(?<where>.+)$   # Captures everything after the last 'WHERE', presumably predicates
            ",
            RegexOptions.IgnoreCase |
            RegexOptions.Singleline |
            RegexOptions.IgnorePatternWhitespace |
            RegexOptions.Compiled
        );



        public List<INode> ParseQuery(string query)
        {
            var explanation = Connector.CallQuery($"EXPLAIN {query}");



            throw new NotImplementedException();
        }


        private static Regex TableFinder = new Regex(@"->.*?\sScan(?:\susing \w+)?\son\s(?<tableName>\w+)(?:\s(?<alias>\w+))?  \(cost=", RegexOptions.Compiled);
        public async Task<List<INode>> GetTables(string query)
        {
            List<Match> matchedRows = await GetExplanationMatches(query, TableFinder);

            int id=0;
            return matchedRows
                .Select(match => new TableReferenceNode(
                    id:         id++,
                    tableName:  match.Groups["tableName"].Value,
                    alias:      (match.Groups["alias"] ?? match.Groups["tableName"]).Value
                ) as INode)
                .ToList();
        }

        private async Task<List<Match>> GetExplanationMatches(string query, Regex regex)
        {
            List<string> explanationRows = await GetPGExplainationRows(query);

            return explanationRows
                .Select(row => regex.Match(row))
                .Where((match) => !string.IsNullOrEmpty(match.Value))
                .ToList();
        }

        private async Task<List<string>> GetPGExplainationRows(string query)
        {
            string explainQuery = $"EXPLAIN {query}";
            var explanation = await Connector.CallQuery(explainQuery);
            var rawRows = explanation.Tables[0].Rows;

            var stringRows = new List<string>();

            foreach(DataRow row in rawRows)
            {
                object queryPlan = row["QUERY PLAN"];
                if(queryPlan == null)
                    throw new NullReferenceException($"\"QUERY PLAN\" not found from running '{explainQuery}' on postgres. Verify the connection");

                string? rowStr = queryPlan.ToString();

                if(!string.IsNullOrEmpty(rowStr))
                    stringRows.Add(rowStr);
            }

            return stringRows;
        }
    }
}
