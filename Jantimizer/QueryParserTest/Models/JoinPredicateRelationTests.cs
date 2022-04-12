using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserTest.Models
{
    [TestClass]
    public class JoinPredicateRelationTests
    {
        #region GetJoinTables From JoinPredicateRelation
        [TestMethod]
        public void Can_GetJoinTables_JoinPredicateRelation_LeftOnly()
        {
            // ARRANGE
            var leftTable = new TableReferenceNode(0, "TableA", "A");
            var rightTable = new TableReferenceNode(1, "TableB", "B");
            var predicate = new JoinPredicateRelation(
                new JoinPredicate(
                    leftTable,
                    "a",
                    rightTable,
                    "b",
                    "b > a",
                    ComparisonType.Type.More
                    )
                );

            // ACT
            var tables = predicate.GetJoinTables(true, false);

            // ASSERT
            Assert.IsTrue(tables.Count == 1);
            Assert.AreEqual(leftTable, tables[0]);
        }

        [TestMethod]
        public void Can_GetJoinTables_JoinPredicateRelation_RightOnly()
        {
            // ARRANGE
            var leftTable = new TableReferenceNode(0, "TableA", "A");
            var rightTable = new TableReferenceNode(1, "TableB", "B");
            var predicate = new JoinPredicateRelation(
                new JoinPredicate(
                    leftTable,
                    "a",
                    rightTable,
                    "b",
                    "b > a",
                    ComparisonType.Type.More
                    )
                );

            // ACT
            var tables = predicate.GetJoinTables(false, true);

            // ASSERT
            Assert.IsTrue(tables.Count == 1);
            Assert.AreEqual(rightTable, tables[0]);
        }

        [TestMethod]
        public void Can_GetJoinTables_JoinPredicateRelation_BothTables()
        {
            // ARRANGE
            var leftTable = new TableReferenceNode(0, "TableA", "A");
            var rightTable = new TableReferenceNode(1, "TableB", "B");
            var predicate = new JoinPredicateRelation(
                new JoinPredicate(
                    leftTable,
                    "a",
                    rightTable,
                    "b",
                    "b > a",
                    ComparisonType.Type.More
                    )
                );

            // ACT
            var tables = predicate.GetJoinTables(true, true);

            // ASSERT
            Assert.IsTrue(tables.Count == 2);
            Assert.AreEqual(leftTable, tables[0]);
            Assert.AreEqual(rightTable, tables[1]);
        }
        #endregion

        #region GetJoinTables From Relations

        [TestMethod]
        public void Can_GetJoinTables_Relations_LeftOnly()
        {
            // ARRANGE
            var leftTable1 = new TableReferenceNode(0, "TableA", "A");
            var rightTable1 = new TableReferenceNode(1, "TableB", "B");
            var predicate = new JoinPredicateRelation(
                new JoinPredicateRelation(
                    new JoinPredicate(
                        leftTable1,
                        "a",
                        rightTable1,
                        "b",
                        "b > a",
                        ComparisonType.Type.More
                        )
                ),
                null,
                RelationType.Type.Predicate);

            // ACT
            var tables = predicate.GetJoinTables(true, false);

            // ASSERT
            Assert.IsTrue(tables.Count == 2);
            Assert.AreEqual(leftTable1, tables[0]);
            Assert.AreEqual(rightTable1, tables[1]);
        }

        [TestMethod]
        public void Can_GetJoinTables_Relations_RightOnly()
        {
            // ARRANGE
            var leftTable2 = new TableReferenceNode(2, "TableC", "C");
            var rightTable2 = new TableReferenceNode(3, "TableD", "D");
            var predicate = new JoinPredicateRelation(
                null,
                new JoinPredicateRelation(
                    new JoinPredicate(
                        leftTable2,
                        "c",
                        rightTable2,
                        "d",
                        "d < c",
                        ComparisonType.Type.Less
                        )
                ),
                RelationType.Type.Predicate);

            // ACT
            var tables = predicate.GetJoinTables(true, false);

            // ASSERT
            Assert.IsTrue(tables.Count == 2);
            Assert.AreEqual(leftTable2, tables[0]);
            Assert.AreEqual(rightTable2, tables[1]);
        }

        [TestMethod]
        public void Can_GetJoinTables_Relations_All()
        {
            // ARRANGE
            var leftTable1 = new TableReferenceNode(0, "TableA", "A");
            var rightTable1 = new TableReferenceNode(1, "TableB", "B");
            var leftTable2 = new TableReferenceNode(2, "TableC", "C");
            var rightTable2 = new TableReferenceNode(3, "TableD", "D");
            var predicate = new JoinPredicateRelation(
                new JoinPredicateRelation(
                    new JoinPredicate(
                        leftTable1,
                        "a",
                        rightTable1,
                        "b",
                        "b > a",
                        ComparisonType.Type.More
                        )
                ),
                new JoinPredicateRelation(
                    new JoinPredicate(
                        leftTable2,
                        "c",
                        rightTable2,
                        "d",
                        "d < c",
                        ComparisonType.Type.Less
                        )
                ),
                RelationType.Type.Predicate);

            // ACT
            var tables = predicate.GetJoinTables(true, false);

            // ASSERT
            Assert.IsTrue(tables.Count == 4);
            Assert.AreEqual(leftTable1, tables[0]);
            Assert.AreEqual(rightTable1, tables[1]);
            Assert.AreEqual(leftTable2, tables[2]);
            Assert.AreEqual(rightTable2, tables[3]);
        }

        #endregion
    }
}
