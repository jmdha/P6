using Microsoft.VisualStudio.TestTools.UnitTesting;

using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System.Text.RegularExpressions;

namespace QueryParsers
{
    [TestClass]
    public class PostgresParserTest
    {
        #region AnalyseExplanationText
        [TestMethod]
        [DataRow(
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
            PostgresParser parser = new PostgresParser();
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
            PostgresParser parser = new PostgresParser();
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
            PostgresParser parser = new PostgresParser();
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

        #region InsertTables

        [TestMethod]
        [DataRow(
     $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
     $"  Join Filter: (a.v1 = b.v2)\n" +
     $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
     $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
     $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
     new string[] { "a", "b" },0)]
        [DataRow(
     $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
     $"  Join Filter: (c.v1 = d.v2)\n" +
     $"  ->  Seq Scan on c  (cost=0.00..1.50 rows=50 width=8)\n" +
     $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
     $"      ->  Seq Scan on d  (cost=0.00..1.10 rows=10 width=8)\n",
     new string[] { "c", "d" }, 0)]
        [DataRow(
     $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
     $"  Join Filter: (c.v1 = d.v2)\n" +
     $"  ->  Seq Scan on c  (cost=0.00..1.50 rows=50 width=8)\n" +
     $"  ->  Seq Scan on e  (cost=0.00..1.50 rows=50 width=8)\n" +
     $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
     $"      ->  Seq Scan on d  (cost=0.00..1.10 rows=10 width=8)\n",
     new string[] { "c", "d", "e" }, 0)]
        public void Can_InsertTables(string explainResults, string[] expTableNames, int randomNumberThatIsNeeded)
        {
            PostgresParser parser = new PostgresParser();
            ParserResult result = new ParserResult();
            parser.InsertTables(explainResults, ref result);

            Assert.AreEqual(expTableNames.Length, result.Tables.Count);
            foreach (string table in expTableNames)
                Assert.IsNotNull(result.Tables[table]);
        }

        #endregion

        #region GetAliasFromRegexMatch

        [TestMethod]
        [DataRow("        ->  Index Scan using comp_cast_type_pkey on comp_cast_type a  (cost=0.13..0.15 rows=1 width=4)", "a")]
        [DataRow("        ->  Index Scan using comp_cast_type_pkey on comp_cast_type other  (cost=0.13..0.15 rows=1 width=4)", "other")]
        [DataRow("        ->  Index Scan using comp_cast_type_pkey on ab ba  (cost=0.13..0.15 rows=1 width=4)", "ba")]
        public void Can_GetAliasFromRegexMatch(string tableString, string expAlias)
        {
            // ARRANGE
            PostgresParser parser = new PostgresParser();
            Match match = PostgresParser.TableFinder.Match(tableString);

            // ACT
            var res = parser.GetAliasFromRegexMatch(match);

            // ASSERT
            Assert.AreEqual(expAlias, res);
        }

        [TestMethod]
        [DataRow("        ->  Index Scan using comp_cast_type_pkey on comp_cast_type  (cost=0.13..0.15 rows=1 width=4)", "comp_cast_type")]
        [DataRow("        ->  Index Scan using comp_cast_type_pkey on comp_cast_type_b  (cost=0.13..0.15 rows=1 width=4)", "comp_cast_type_b")]
        [DataRow("        ->  Index Scan using comp_cast_type_pkey on ab  (cost=0.13..0.15 rows=1 width=4)", "ab")]
        public void Can_GetAliasFromRegexMatch_GetsTableIfNoAlias(string tableString, string expAlias)
        {
            // ARRANGE
            PostgresParser parser = new PostgresParser();
            Match match = PostgresParser.TableFinder.Match(tableString);

            // ACT
            var res = parser.GetAliasFromRegexMatch(match);

            // ASSERT
            Assert.AreEqual(expAlias, res);
        }

        #endregion

        #region ConditionTest
        [TestMethod]
        [DataRow(
             $"        ->  Index Scan using comp_cast_type_pkey on comp_cast_type a  (cost=0.13..0.15 rows=1 width=4)\n" +
             $"              Index Cond: (v1 = b.v2)\n",
			 ComparisonType.Type.Equal,
             "a.v1 = b.v2"
        )]
		[DataRow(
             $"        ->  Index Scan using comp_cast_type_pkey on comp_cast_type a  (cost=0.13..0.15 rows=1 width=4)\n" +
             $"              Index Cond: (v1 > b.v2)\n",
			 ComparisonType.Type.More,
             "a.v1 > b.v2"
        )]
		[DataRow(
             $"        ->  Index Scan using comp_cast_type_pkey on comp_cast_type a  (cost=0.13..0.15 rows=1 width=4)\n" +
             $"              Index Cond: (v1 < b.v2)\n",
			 ComparisonType.Type.Less,
             "a.v1 < b.v2"
        )]
		[DataRow(
             $"        ->  Index Scan using comp_cast_type_pkey on comp_cast_type a  (cost=0.13..0.15 rows=1 width=4)\n" +
             $"              Index Cond: (v1 >= b.v2)\n",
			 ComparisonType.Type.EqualOrMore,
             "a.v1 >= b.v2"
        )]
		[DataRow(
             $"        ->  Index Scan using comp_cast_type_pkey on comp_cast_type a  (cost=0.13..0.15 rows=1 width=4)\n" +
             $"              Index Cond: (v1 <= b.v2)\n",
			 ComparisonType.Type.EqualOrLess,
             "a.v1 <= b.v2"
        )]
        public void InsertConditionsTest(string explainResults, ComparisonType.Type type, string predicate) {
			PostgresParser parser = new PostgresParser();
			ParserResult result = new ParserResult();
			result.Tables.Add("a", new TableReferenceNode(0, "a", "a"));
			result.Tables.Add("b", new TableReferenceNode(0, "b", "b"));
            parser.InsertConditions(explainResults, ref result);
            Assert.AreEqual(1, result.Joins.Count);
			Assert.IsNotNull(result.Joins[0].Relation);
			Assert.IsNotNull(result.Joins[0].Relation.LeafPredicate);
			Assert.IsNull(result.Joins[0].Relation.LeftRelation);
			Assert.IsNull(result.Joins[0].Relation.RightRelation);
            Assert.AreEqual(predicate, result.Joins[0].Predicate);
			Assert.AreEqual("a", result.Joins[0].Relation!.LeafPredicate!.LeftTable.Alias);
			Assert.AreEqual("b", result.Joins[0].Relation!.LeafPredicate!.RightTable.Alias);
			Assert.AreEqual("v1", result.Joins[0].Relation!.LeafPredicate!.LeftAttribute);
			Assert.AreEqual("v2", result.Joins[0].Relation!.LeafPredicate!.RightAttribute);
			Assert.AreEqual(type, result.Joins[0].Relation!.LeafPredicate!.ComType);
        }
        #endregion

		#region FilterTest
		[TestMethod]
        [DataRow(
			$"->  Index Scan using title_pkey on title a  (cost=0.43..0.85 rows=1 width=21)\n" +
            $"  Filter: (v1 = 0)\n",
			"a",
			ComparisonType.Type.Equal,
            "v1",
			"0"
        )]
		[DataRow(
			$"->  Index Scan using title_pkey on title b  (cost=0.43..0.85 rows=1 width=21)\n" +
            $"  Filter: (v2 <= 1)\n",
			"b",
			ComparisonType.Type.EqualOrLess,
            "v2",
			"1"
        )]
		[DataRow(
			$"->  Index Scan using title_pkey on title c  (cost=0.43..0.85 rows=1 width=21)\n" +
            $"  Filter: (v3 >= 2)\n",
			"c",
			ComparisonType.Type.EqualOrMore,
            "v3",
			"2"
        )]
		[DataRow(
			$"->  Index Scan using title_pkey on title d  (cost=0.43..0.85 rows=1 width=21)\n" +
            $"  Filter: (v4 < 3)\n",
			"d",
			ComparisonType.Type.Less,
            "v4",
			"3"
        )]
		[DataRow(
			$"->  Index Scan using title_pkey on title e  (cost=0.43..0.85 rows=1 width=21)\n" +
            $"  Filter: (v5 > 4)\n",
			"e",
			ComparisonType.Type.More,
            "v5",
			"4"
        )]
        public void InsertFiltersTest(string explainResults, string tableAlias, ComparisonType.Type type, string attribute, string constant) {
			PostgresParser parser = new PostgresParser(null);
			ParserResult result = new ParserResult();
			TableReferenceNode tRefNode = new TableReferenceNode(0, tableAlias, tableAlias);
			result.Tables.Add(tableAlias, tRefNode);
            parser.InsertFilters(explainResults, ref result);
			Assert.IsNotNull(result.Tables[tableAlias]);
			Assert.AreEqual(1, result.Tables[tableAlias].Filters.Count);
			Assert.AreEqual(tRefNode, result.Tables[tableAlias].Filters[0].TableReference);
			Assert.AreEqual(type, result.Tables[tableAlias].Filters[0].ComType);
			Assert.AreEqual(attribute, result.Tables[tableAlias].Filters[0].AttributeName);
			Assert.AreEqual(constant, result.Tables[tableAlias].Filters[0].Constant);
        }
		#endregion
    }
}