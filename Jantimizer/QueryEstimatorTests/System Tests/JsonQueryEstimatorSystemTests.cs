using Histograms.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryEstimator;
using QueryEstimatorTests.Stubs;
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
        //      Then gives: 3000 (from above) * 50 (total rows within bounds of C.v) = 150.000 total rows estimation
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
            Assert.AreEqual(150000UL, result.EstimatedCardinality);
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
        // "(A JOIN B ON A.v = B.v)" : Count(A.v) * Count(B.v) = 20 * 30 = 600
        // Followed by "... JOIN C ON B.v > C.v",
        //      flipped to most significant table: "...  JOIN C ON C.v < B.v"
        //      Then gives: 600 (from above) * 20 (total rows within bounds of C.v) = 12000 total rows estimation
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
            Assert.AreEqual(12000UL, result.EstimatedCardinality);
        }

        [TestMethod]
        // SELECT * FROM (A JOIN B ON A.v = B.v) JOIN C ON B.v = C.v
        // "(A JOIN B ON A.v = B.v)" : 10 + 10 + 10 = 30
        // Followed by "... JOIN C ON B.v = C.v",
        //      flipped to most significant table: "...  JOIN C ON C.v = B.v"
        //      Then gives: 30 (from above) * 0 (total rows within bounds of C.v) = 0 total rows estimation
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
            TestHistogramManager testManager = new TestHistogramManager();
            TestDataGathereStub testGatherer = new TestDataGathereStub();

            // Add A table
            var tabelAttr1Histogram = new HistogramEquiDepth(tableAttr1.Table.TableName, tableAttr1.Attribute, 10);
            var tabelAttr1HistoValues = GetTableAttr1Values();
            var tabelAttr1AttributeData = new AttributeData(tableAttr1, tabelAttr1HistoValues, TypeCode.Int32);
            tabelAttr1Histogram.GenerateSegmentationsFromSortedGroups(tabelAttr1HistoValues);
            testManager.AddHistogram(tabelAttr1Histogram);

            // Add B table
            var tabelAttr2Histogram = new HistogramEquiDepth(tableAttr2.Table.TableName, tableAttr2.Attribute, 10);
            var tabelAttr2HistoValues = GetTableAttr2Values();
            var tabelAttr2AttributeData = new AttributeData(tableAttr2, tabelAttr2HistoValues, TypeCode.Int32);
            tabelAttr2Histogram.GenerateSegmentationsFromSortedGroups(tabelAttr2HistoValues);
            testManager.AddHistogram(tabelAttr2Histogram);

            // Add C table
            var tabelAttr3Histogram = new HistogramEquiDepth(tableAttr3.Table.TableName, tableAttr3.Attribute, 10);
            var tabelAttr3HistoValues = GetTableAttr3Values();
            var tabelAttr3AttributeData = new AttributeData(tableAttr3, tabelAttr3HistoValues, TypeCode.Int32);
            tabelAttr3Histogram.GenerateSegmentationsFromSortedGroups(tabelAttr3HistoValues);
            testManager.AddHistogram(tabelAttr3Histogram);

            testGatherer.Data.Add(tableAttr1, tabelAttr1AttributeData);
            testGatherer.Data.Add(tableAttr2, tabelAttr2AttributeData);
            testGatherer.Data.Add(tableAttr3, tabelAttr3AttributeData);

            var segmentComparer = new SegmentationComparisonCalculator(testGatherer);
            segmentComparer.DoHistogramComparisons(testManager.Histograms.Values.ToList()).Wait();

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
