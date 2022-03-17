using Microsoft.VisualStudio.TestTools.UnitTesting;

using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;

namespace QueryParsers
{
    [TestClass]
    public class PostgresParserTest
    {
        [TestMethod]
        [DataRow (
            //$"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n  Join Filter: (a.v > b.v)\n  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n        ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)",
             $"Nested Loop  (cost=0.00..10.13 rows=167 width=16)\n" +
             $"  Join Filter: (a.v1 > b.v2)\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n",
             ComparisonType.Type.More,
             "(a.v1 > b.v2)")]
        public void AnalyseExplanationTextTest(string explainResults, ComparisonType.Type type, string predicate)
        {
            ParserResult result = PostgresParser.AnalyseExplanationText(explainResults);
            
            Assert.AreEqual(1, result.Joins.Count);
            Assert.AreEqual(predicate, result.Joins[0].Predicate);
            Assert.IsNotNull(result.Joins[0].Relation);
            Assert.IsNotNull(result.Joins[0].Relation.LeafPredicate);
            Assert.IsNull(result.Joins[0].Relation.LeftRelation);
            Assert.IsNull(result.Joins[0].Relation.RightRelation);
            if (result.Joins[0].Relation != null && result.Joins[0].Relation.LeafPredicate != null && result.Joins[0] != null) {
                Assert.AreEqual("a", result.Joins[0].Relation.LeafPredicate.LeftTable.Alias);
                Assert.AreEqual("b", result.Joins[0].Relation.LeafPredicate.RightTable.Alias);
                Assert.AreEqual("v1", result.Joins[0].Relation.LeafPredicate.LeftAttribute);
                Assert.AreEqual("v2", result.Joins[0].Relation.LeafPredicate.RightAttribute);
            }
            
        }
    }
}