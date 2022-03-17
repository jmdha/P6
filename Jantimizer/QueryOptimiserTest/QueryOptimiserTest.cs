using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using QueryOptimiser;
using Histograms;
using Histograms.Managers;
using QueryParser;
using QueryParser.QueryParsers;
using QueryParser.Models;
using DatabaseConnector;

namespace QueryOptimiserTest;

[TestClass]
public class QueryOptimiserTest
{
    [TestMethod]
    [DataRow(
    0,                                                                                                  // testID
    new int[] { 0, 1 },                                                                                 // expectedNodeOrder
    new string[] { "T1", "T2", "T3" },                                                                     // name
    new int[] { 10, 10, 10 },                                                                           // depth
    new int[] { 0, 0, 0 },                                                                              // min
    new int[] { 100, 100, 100 },                                                                        // max
    new int[] { 0, 1 },                                                                                 // nodeId
    new ComparisonType.Type[] { ComparisonType.Type.Equal, ComparisonType.Type.Equal },                 // nodeComparisonType
    new string[] { "T1", "T2" },                                                                          // nodeLeftTable
    new string[] { "T2", "T3" },                                                                          // nodeRightTable
    new string[] { "ID", "ID" },                                                                        // nodeLeftAttribute
    new string[] { "ID", "ID" },                                                                        // nodeRightAttribute
    new string[] { "T1 = T2", "T2 = T3" }                                                                   // nodeCondition
    )]
    [DataRow(
    1,                                                                                                  // testID
    new int[] { 0, 1 },                                                                                 // expectedNodeOrder
    new string[] { "T1", "T2", "T3" },                                                                     // name
    new int[] { 10, 10, 10 },                                                                           // depth
    new int[] { 0, 0, 0 },                                                                              // min
    new int[] { 50, 100, 150 },                                                                         // max
    new int[] { 0, 1 },                                                                                 // nodeId
    new ComparisonType.Type[] { ComparisonType.Type.Equal, ComparisonType.Type.Equal },                 // nodeComparisonType
    new string[] { "T1", "T2" },                                                                          // nodeLeftTable
    new string[] { "T2", "T3" },                                                                          // nodeRightTable
    new string[] { "ID", "ID" },                                                                        // nodeLeftAttribute
    new string[] { "ID", "ID" },                                                                        // nodeRightAttribute
    new string[] { "T1 < T2", "T2 < T3" }                                                                   // nodeCondition
    )]
    [DataRow(
    2,                                                                                                  // testID
    new int[] { 1, 0 },                                                                                 // expectedNodeOrder
    new string[] { "T1", "T2", "T3" },                                                                     // name
    new int[] { 10, 10, 10 },                                                                           // depth
    new int[] { 0, 0, 0 },                                                                              // min
    new int[] { 100, 100, 50 },                                                                         // max
    new int[] { 0, 1 },                                                                                 // nodeId
    new ComparisonType.Type[] { ComparisonType.Type.Equal, ComparisonType.Type.Equal },                 // nodeComparisonType
    new string[] { "T1", "T2" },                                                                          // nodeLeftTable
    new string[] { "T2", "T3" },                                                                          // nodeRightTable
    new string[] { "ID", "ID" },                                                                        // nodeLeftAttribute
    new string[] { "ID", "ID" },                                                                        // nodeRightAttribute
    new string[] { "T1 = T2", "T2 = T3" }                                                                   // nodeCondition
    )]
    [DataRow(
    3,                                                                                                  // testID
    new int[] { 0, 1 },                                                                                 // expectedNodeOrder
    new string[] { "T1", "T2", "T3" },                                                                     // name
    new int[] { 10, 10, 10 },                                                                           // depth
    new int[] { 0, 0, 0 },                                                                              // min
    new int[] { 150, 100, 50 },                                                                         // max
    new int[] { 1, 0 },                                                                                 // nodeId
    new ComparisonType.Type[] { ComparisonType.Type.More, ComparisonType.Type.More },                   // nodeComparisonType
    new string[] { "T1", "T2" },                                                                          // nodeLeftTable
    new string[] { "T2", "T3" },                                                                          // nodeRightTable
    new string[] { "ID", "ID" },                                                                        // nodeLeftAttribute
    new string[] { "ID", "ID" },                                                                        // nodeRightAttribute
    new string[] { "T1 > T2", "T2 > T3" }                                                                   // nodeCondition
    )]
    [DataRow(
    4,                                                                                                  // testID
    new int[] { 0, 1 },                                                                                 // expectedNodeOrder
    new string[] { "T1", "T2", "T3" },                                                                     // name
    new int[] { 10, 10, 10 },                                                                           // depth
    new int[] { 0, 0, 0 },                                                                              // min
    new int[] { 50, 100, 150 },                                                                         // max
    new int[] { 1, 0 },                                                                                 // nodeId
    new ComparisonType.Type[] { ComparisonType.Type.Less, ComparisonType.Type.Less },                   // nodeComparisonType
    new string[] { "T2", "T1" },                                                                          // nodeLeftTable
    new string[] { "T3", "T2" },                                                                          // nodeRightTable
    new string[] { "ID", "ID" },                                                                        // nodeLeftAttribute
    new string[] { "ID", "ID" },                                                                        // nodeRightAttribute
    new string[] { "T2 < T3", "T1 < T2" }                                                                   // nodeCondition
    )]

    public void OptimiseJoinQueryEqual(int testID, int[] expectedNodeOrder, string[] name, int[] depth, int[] min, int[] max, int[] nodeId, ComparisonType.Type[] nodeComparisonType, string[] nodeLeftTable, string[] nodeRightTable, string[] nodeLeftAttribute, string[] nodeRightAttribute, string[] nodeCondition)
    {
        // Arrange
        var histogramManager = new PostgresEquiDepthHistogramManager("SomeConnectionString", 10);
        for (int i = 0; i < depth.Length; i++)
            histogramManager.AddHistogram(Utilities.CreateIncreasingHistogram(
                name[i].ToString(),
                "ID",
                depth[i],
                min[i],
                max[i]
            ));
        
        List<INode> queryNodes = new List<INode>();
        for (int i = 0; i < nodeId.Length; i++) 
            queryNodes.Add(new JoinNode(
                nodeId[i], 
                nodeComparisonType[i],
                nodeLeftTable[i],
                nodeLeftAttribute[i],
                nodeRightTable[i],
                nodeRightAttribute[i],
                nodeCondition[i]));
        
        var queryOptimiser = new QueryOptimiserEquiDepth(histogramManager);

        // Act
        var resultNodes = queryOptimiser.OptimiseQuery(queryNodes);

        // Assert
        for (int i = 0; i < expectedNodeOrder.Length; i++) {
            Assert.AreEqual(expectedNodeOrder[i], resultNodes[i].Node.Id);
        }
    }
}