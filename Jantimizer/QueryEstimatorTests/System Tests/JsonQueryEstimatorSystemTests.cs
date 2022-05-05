using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryEstimator;
using QueryEstimatorTests.Stubs;
using Milestoner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.System_Tests
{
    [TestClass]
    public class JsonQueryEstimatorSystemTests
    {
        [TestMethod]
        // SELECT * FROM (A JOIN B ON A.v < B.v) JOIN C ON B.v < C.v
        // "(A JOIN B ON A.v < B.v)" : (60 * 10) + (60 * 10) + (60 * 10) + (50 * 10) + (40 * 10) + (30 * 10) = 3000
        // Followed by "... JOIN C ON B.v < C.v",
        //      flipped to most significant table: "...  JOIN C ON C.v >= B.v"
        //      Then gives: 3000 (from above) * 40 (total rows within bounds of C.v) = 120.000 total rows estimation
        public void Can_GetQueryEstimation_1()
        {
            // ARRANGE
            TableAttribute tableAttr1 = new TableAttribute("A", "v");
            TableAttribute tableAttr2 = new TableAttribute("B", "v");
            TableAttribute tableAttr3 = new TableAttribute("C", "v");

            IQueryEstimator<JsonQuery> estimator = GetBaseEstimator(tableAttr1, tableAttr2, tableAttr3);
            var joinList = new List<JoinNode>() { 
                new JoinNode(new List<JoinPredicate>(){ 
                    new JoinPredicate(
                        new PredicateNode(tableAttr1),
                        new PredicateNode(tableAttr2),
                        "<")
                }),
                new JoinNode(new List<JoinPredicate>(){
                    new JoinPredicate(
                        new PredicateNode(tableAttr2),
                        new PredicateNode(tableAttr3),
                        "<")
                })
            };
            var query = new JsonQuery(joinList, "SELECT * FROM (A JOIN B ON A.v < B.v) JOIN C ON B.v < C.v", true);

            // ACT
            var result = estimator.GetQueryEstimation(query);

            // ASSERT
            Assert.AreEqual(120000UL, result.EstimatedCardinality);
        }

        [TestMethod]
        // SELECT * FROM (A JOIN B ON A.v < B.v) JOIN C ON B.v > C.v
        // "(A JOIN B ON A.v < B.v)" : (60 * 10) + (60 * 10) + (60 * 10) + (50 * 10) + (40 * 10) + (30 * 10) = 3000
        // Followed by "... JOIN C ON B.v > C.v",
        //      flipped to most significant table: "...  JOIN C ON C.v < B.v"
        //      Then gives: 3000 (from above) * 20 (total rows within bounds of C.v) = 60.000 total rows estimation
        public void Can_GetQueryEstimation_2()
        {
            // ARRANGE
            TableAttribute tableAttr1 = new TableAttribute("A", "v");
            TableAttribute tableAttr2 = new TableAttribute("B", "v");
            TableAttribute tableAttr3 = new TableAttribute("C", "v");

            IQueryEstimator<JsonQuery> estimator = GetBaseEstimator(tableAttr1, tableAttr2, tableAttr3);
            var joinList = new List<JoinNode>() {
                new JoinNode(new List<JoinPredicate>(){
                    new JoinPredicate(
                        new PredicateNode(tableAttr1),
                        new PredicateNode(tableAttr2),
                        "<")
                }),
                new JoinNode(new List<JoinPredicate>(){
                    new JoinPredicate(
                        new PredicateNode(tableAttr2),
                        new PredicateNode(tableAttr3),
                        ">")
                })
            };
            var query = new JsonQuery(joinList, "SELECT * FROM (A JOIN B ON A.v < B.v) JOIN C ON B.v > C.v", true);

            // ACT
            var result = estimator.GetQueryEstimation(query);

            // ASSERT
            Assert.AreEqual(60000UL, result.EstimatedCardinality);
        }

        [TestMethod]
        // SELECT * FROM (A JOIN B ON A.v = B.v) JOIN C ON B.v > C.v
        // "(A JOIN B ON A.v = B.v)" : Count(A.v) * Count(B.v) = 20 * 20 = 400
        // Followed by "... JOIN C ON B.v > C.v",
        //      flipped to most significant table: "...  JOIN C ON C.v < B.v"
        //      Then gives: 400 (from above) * 20 (total rows within bounds of C.v) = 8000 total rows estimation
        public void Can_GetQueryEstimation_3()
        {
            // ARRANGE
            TableAttribute tableAttr1 = new TableAttribute("A", "v");
            TableAttribute tableAttr2 = new TableAttribute("B", "v");
            TableAttribute tableAttr3 = new TableAttribute("C", "v");

            IQueryEstimator<JsonQuery> estimator = GetBaseEstimator(tableAttr1, tableAttr2, tableAttr3);
            var joinList = new List<JoinNode>() {
                new JoinNode(new List<JoinPredicate>(){
                    new JoinPredicate(
                        new PredicateNode(tableAttr1),
                        new PredicateNode(tableAttr2),
                        "=")
                }),
                new JoinNode(new List<JoinPredicate>(){
                    new JoinPredicate(
                        new PredicateNode(tableAttr2),
                        new PredicateNode(tableAttr3),
                        ">")
                })
            };
            var query = new JsonQuery(joinList, "SELECT * FROM (A JOIN B ON A.v = B.v) JOIN C ON B.v > C.v", true);

            // ACT
            var result = estimator.GetQueryEstimation(query);

            // ASSERT
            Assert.AreEqual(8000UL, result.EstimatedCardinality);
        }

        [TestMethod]
        // SELECT * FROM (A JOIN B ON A.v = B.v) JOIN C ON B.v = C.v
        // "(A JOIN B ON A.v = B.v)" : 20 * 20 = 400
        // Followed by "... JOIN C ON B.v = C.v",
        //      flipped to most significant table: "...  JOIN C ON C.v = B.v"
        //      Then gives: 400 (from above) * 0 (total rows within bounds of C.v) = 0 total rows estimation
        public void Can_GetQueryEstimation_4()
        {
            // ARRANGE
            TableAttribute tableAttr1 = new TableAttribute("A", "v");
            TableAttribute tableAttr2 = new TableAttribute("B", "v");
            TableAttribute tableAttr3 = new TableAttribute("C", "v");

            IQueryEstimator<JsonQuery> estimator = GetBaseEstimator(tableAttr1, tableAttr2, tableAttr3);
            var joinList = new List<JoinNode>() {
                new JoinNode(new List<JoinPredicate>(){
                    new JoinPredicate(
                        new PredicateNode(tableAttr1),
                        new PredicateNode(tableAttr2),
                        "=")
                }),
                new JoinNode(new List<JoinPredicate>(){
                    new JoinPredicate(
                        new PredicateNode(tableAttr2),
                        new PredicateNode(tableAttr3),
                        "=")
                })
            };
            var query = new JsonQuery(joinList, "SELECT * FROM (A JOIN B ON A.v = B.v) JOIN C ON B.v = C.v", true);

            // ACT
            var result = estimator.GetQueryEstimation(query);

            // ASSERT
            Assert.AreEqual(0UL, result.EstimatedCardinality);
        }

        #region Private Test Methods

        private IQueryEstimator<JsonQuery> GetBaseEstimator(TableAttribute tableAttr1, TableAttribute tableAttr2, TableAttribute tableAttr3)
        {
            TestMilestonerManager testManager = new TestMilestonerManager();

            // Add A table
            foreach(var value in GetTableAttr1Values())
                testManager.AddMilestonesFromValueCountManual(tableAttr1, value.Value, value.Value, 10);

            // Add B table
            foreach (var value in GetTableAttr2Values())
                testManager.AddMilestonesFromValueCountManual(tableAttr2, value.Value, value.Value, 10);

            // Add C table
            foreach (var value in GetTableAttr3Values())
                testManager.AddMilestonesFromValueCountManual(tableAttr3, value.Value, value.Value, 10);

            testManager.Comparer.DoMilestoneComparisons();

            return new JsonQueryEstimator(testManager, 10);
        }

        private List<ValueCount> GetTableAttr1Values()
        {
            var list = new List<ValueCount>();
            list.Add(new ValueCount(0, 10));
            list.Add(new ValueCount(10, 10));
            list.Add(new ValueCount(20, 10));
            list.Add(new ValueCount(30, 10));
            list.Add(new ValueCount(40, 10));
            list.Add(new ValueCount(50, 10));
            return list;
        }

        private List<ValueCount> GetTableAttr2Values()
        {
            var list = new List<ValueCount>();
            list.Add(new ValueCount(30, 10));
            list.Add(new ValueCount(40, 10));
            list.Add(new ValueCount(50, 10));
            list.Add(new ValueCount(60, 10));
            list.Add(new ValueCount(70, 10));
            list.Add(new ValueCount(80, 10));
            return list;
        }

        private List<ValueCount> GetTableAttr3Values()
        {
            var list = new List<ValueCount>();
            list.Add(new ValueCount(10, 10));
            list.Add(new ValueCount(20, 10));
            list.Add(new ValueCount(90, 10));
            list.Add(new ValueCount(100, 10));
            list.Add(new ValueCount(110, 10));
            list.Add(new ValueCount(120, 10));
            return list;
        }

        #endregion
    }
}
