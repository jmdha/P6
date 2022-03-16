using Microsoft.VisualStudio.TestTools.UnitTesting;

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
             $"  Join Filter: ((a.v > b.v) OR (a.v < b.v))\n" +
             $"  ->  Seq Scan on b  (cost=0.00..1.50 rows=50 width=8)\n" +
             $"  ->  Materialize(cost=0.00..1.15 rows=10 width=8)\n" +
             $"      ->  Seq Scan on a  (cost=0.00..1.10 rows=10 width=8)\n"

            )]
        public void AnalyseExplanationTextTest(string explainResults)
        {
            var result = PostgresParser.AnalyseExplanationText(explainResults);
            Assert.AreEqual(explainResults, result);
        }
    }
}