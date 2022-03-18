using Microsoft.VisualStudio.TestTools.UnitTesting;

using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;

namespace QueryParsers
{
    [TestClass]
    public class PostgresParserTest
    {
        #region AnalyseExplanation
        #region JoinTest
        [TestMethod]
        [DataRow (
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: (a.v1 = b.v2)\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.Equal,
             "(a.v1 = b.v2)")]
        [DataRow(
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: (a.v1 < b.v2)\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.Less,
             "(a.v1 < b.v2)")]
        [DataRow(
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: (a.v1 > b.v2)\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.More,
             "(a.v1 > b.v2)")]
        [DataRow(
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: (a.v1 >= b.v2)\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.EqualOrMore,
             "(a.v1 >= b.v2)")]
        [DataRow(
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: (a.v1 <= b.v2)\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.EqualOrLess,
             "(a.v1 <= b.v2)")]
        public void AnalyseExplanationSingleJoinTest(string explainResults, ComparisonType.Type type, string predicate)
        {
        	PostgresParser parser = new PostgresParser(new DatabaseConnector.Connectors.PostgreSqlConnector(new Tools.Models.ConnectionProperties()));
            ParserResult result = parser.AnalyseExplanationText(explainResults);
            
            Assert.AreEqual(1, result.Joins.Count);
            Assert.AreEqual(predicate, result.Joins[0].Predicate);
            Assert.IsNotNull(result.Joins[0].Relation);
            Assert.IsNotNull(result.Joins[0].Relation.LeafPredicate);
            Assert.IsNull(result.Joins[0].Relation.LeftRelation);
            Assert.IsNull(result.Joins[0].Relation.RightRelation);
            Assert.AreEqual("a", result.Joins[0].Relation.LeafPredicate!.LeftTable.Alias);
            Assert.AreEqual("b", result.Joins[0].Relation.LeafPredicate!.RightTable.Alias);
            Assert.AreEqual("v1", result.Joins[0].Relation.LeafPredicate!.LeftAttribute);
            Assert.AreEqual("v2", result.Joins[0].Relation.LeafPredicate!.RightAttribute);
            Assert.AreEqual(type, result.Joins[0].Relation.LeafPredicate!.ComType);
        }

        [TestMethod]
        [DataRow(
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: ((a.v1 <= b.v2) AND (a.v1 >= b.v2))\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.EqualOrLess,
             ComparisonType.Type.EqualOrMore,
             "((a.v1 <= b.v2) AND (a.v1 >= b.v2))",
             "a.v1 <= b.v2",
             "a.v1 >= b.v2")]
        [DataRow(
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: ((a.v1 = b.v2) AND (a.v1 = b.v2))\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.Equal,
             ComparisonType.Type.Equal,
             "((a.v1 = b.v2) AND (a.v1 = b.v2))",
             "a.v1 = b.v2",
             "a.v1 = b.v2")]
        public void AnalyseExplanationSingleAndTest(string explainResults, ComparisonType.Type leftType, ComparisonType.Type rightType, string predicate, string leftPredicate, string rightPredicate)
        {
			PostgresParser parser = new PostgresParser(new DatabaseConnector.Connectors.PostgreSqlConnector(new Tools.Models.ConnectionProperties()));
            ParserResult result = parser.AnalyseExplanationText(explainResults);

            Assert.AreEqual(1, result.Joins.Count);
            Assert.AreEqual(predicate, result.Joins[0].Predicate);
            Assert.IsNotNull(result.Joins[0].Relation);
            Assert.IsNotNull(result.Joins[0].Relation.LeftRelation);
            Assert.IsNotNull(result.Joins[0].Relation.RightRelation);
            Assert.IsNotNull(result.Joins[0].Relation.LeftRelation!.LeafPredicate);
            Assert.IsNotNull(result.Joins[0].Relation.RightRelation!.LeafPredicate);
            Assert.AreEqual("a", result.Joins[0].Relation.LeftRelation!.LeafPredicate!.LeftTable.Alias);
            Assert.AreEqual("b", result.Joins[0].Relation.LeftRelation!.LeafPredicate!.RightTable.Alias);
            Assert.AreEqual("v1", result.Joins[0].Relation.LeftRelation!.LeafPredicate!.LeftAttribute);
            Assert.AreEqual("v2", result.Joins[0].Relation.LeftRelation!.LeafPredicate!.RightAttribute);
            Assert.AreEqual(leftType, result.Joins[0].Relation.LeftRelation!.LeafPredicate!.ComType);
            Assert.AreEqual(leftPredicate, result.Joins[0].Relation.LeftRelation!.LeafPredicate!.Condition);
            Assert.AreEqual("a", result.Joins[0].Relation.RightRelation!.LeafPredicate!.LeftTable.Alias);
            Assert.AreEqual("b", result.Joins[0].Relation.RightRelation!.LeafPredicate!.RightTable.Alias);
            Assert.AreEqual("v1", result.Joins[0].Relation.RightRelation!.LeafPredicate!.LeftAttribute);
            Assert.AreEqual("v2", result.Joins[0].Relation.RightRelation!.LeafPredicate!.RightAttribute);
            Assert.AreEqual(rightType, result.Joins[0].Relation.RightRelation!.LeafPredicate!.ComType);
            Assert.AreEqual(rightPredicate, result.Joins[0].Relation.RightRelation!.LeafPredicate!.Condition);
        }

        [TestMethod]
        [DataRow(
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: ((a.v1 <= b.v2) OR (a.v1 >= b.v2))\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.EqualOrLess,
             ComparisonType.Type.EqualOrMore,
             "((a.v1 <= b.v2) OR (a.v1 >= b.v2))",
             "a.v1 <= b.v2",
             "a.v1 >= b.v2")]
        [DataRow(
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: ((a.v1 = b.v2) OR (a.v1 = b.v2))\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.Equal,
             ComparisonType.Type.Equal,
             "((a.v1 = b.v2) OR (a.v1 = b.v2))",
             "a.v1 = b.v2",
             "a.v1 = b.v2")]
        public void AnalyseExplanationSingleOrTest(string explainResults, ComparisonType.Type leftType, ComparisonType.Type rightType, string predicate, string leftPredicate, string rightPredicate)
        {
			PostgresParser parser = new PostgresParser(new DatabaseConnector.Connectors.PostgreSqlConnector(new Tools.Models.ConnectionProperties()));
            ParserResult result = parser.AnalyseExplanationText(explainResults);

            Assert.AreEqual(1, result.Joins.Count);
            Assert.AreEqual(predicate, result.Joins[0].Predicate);
            Assert.IsNotNull(result.Joins[0].Relation);
            Assert.IsNotNull(result.Joins[0].Relation.LeftRelation);
            Assert.IsNotNull(result.Joins[0].Relation.RightRelation);
            Assert.IsNotNull(result.Joins[0].Relation.LeftRelation!.LeafPredicate);
            Assert.IsNotNull(result.Joins[0].Relation.RightRelation!.LeafPredicate);
            Assert.AreEqual("a", result.Joins[0].Relation.LeftRelation!.LeafPredicate!.LeftTable.Alias);
            Assert.AreEqual("b", result.Joins[0].Relation.LeftRelation!.LeafPredicate!.RightTable.Alias);
            Assert.AreEqual("v1", result.Joins[0].Relation.LeftRelation!.LeafPredicate!.LeftAttribute);
            Assert.AreEqual("v2", result.Joins[0].Relation.LeftRelation!.LeafPredicate!.RightAttribute);
            Assert.AreEqual(leftType, result.Joins[0].Relation.LeftRelation!.LeafPredicate!.ComType);
            Assert.AreEqual(leftPredicate, result.Joins[0].Relation.LeftRelation!.LeafPredicate!.Condition);
            Assert.AreEqual("a", result.Joins[0].Relation.RightRelation!.LeafPredicate!.LeftTable.Alias);
            Assert.AreEqual("b", result.Joins[0].Relation.RightRelation!.LeafPredicate!.RightTable.Alias);
            Assert.AreEqual("v1", result.Joins[0].Relation.RightRelation!.LeafPredicate!.LeftAttribute);
            Assert.AreEqual("v2", result.Joins[0].Relation.RightRelation!.LeafPredicate!.RightAttribute);
            Assert.AreEqual(rightType, result.Joins[0].Relation.RightRelation!.LeafPredicate!.ComType);
            Assert.AreEqual(rightPredicate, result.Joins[0].Relation.RightRelation!.LeafPredicate!.Condition);
        }
        #endregion
        #region ConditionTest
        [TestMethod]
        [DataRow(
             $"        ->  Index Scan using comp_cast_type_pkey on comp_cast_type a  (cost=0.13..0.15 rows=1 width=4)\n" +
             $"              Index Cond: (v1 = b.v2)\n",
             "a.v1 = b.v2"
        )]
        public void AnalyseExplanationConditionTest(string explainResults, string predicate) {
			PostgresParser parser = new PostgresParser(new DatabaseConnector.Connectors.PostgreSqlConnector(new Tools.Models.ConnectionProperties()));
			ParserResult result = new ParserResult();
			result.Tables.Add("a", new TableReferenceNode(0, "a", "a"));
			result.Tables.Add("b", new TableReferenceNode(0, "b", "b"));
            parser.InsertConditions(explainResults, ref result);
            Assert.AreEqual(1, result.Joins.Count);
            Assert.AreEqual(predicate, result.Joins[0].Predicate);
        }
        #endregion
        #endregion
    }
}