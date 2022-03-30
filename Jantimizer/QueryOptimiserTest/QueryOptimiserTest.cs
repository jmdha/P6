using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using QueryOptimiser;
using Histograms.Models;
using Histograms.Managers;
using QueryParser;
using QueryParser.QueryParsers;
using QueryParser.Models;
using DatabaseConnector;
using Tools.Models;

namespace QueryOptimiserTest;

[TestClass]
public class QueryOptimiserTest
{
    private static IEnumerable<object[]> OptimiseJoinQueryEqualData()
    {
        List<List<HistogramEquiDepth>> histograms = new List<List<HistogramEquiDepth>>();   
        histograms.AddRange(new List<List<HistogramEquiDepth>>(){
            Utilities.CreateIncreasingHistograms(
                3,
                new int[]{ 10, 10, 10 },
                new int[]{ 0, 0, 0 },
                new int[]{ 100, 100, 100 }
            ),
            Utilities.CreateIncreasingHistograms(
                3,
                new int[]{ 10, 10, 10 },
                new int[]{ 0, 0, 0 },
                new int[]{ 50, 100, 150 }
            ),
            Utilities.CreateIncreasingHistograms(
                3,
                new int[]{ 10, 10, 10 },
                new int[]{ 0, 0, 0 },
                new int[]{ 150, 100, 50 }
            ),
            Utilities.CreateIncreasingHistograms(
                3,
                new int[]{ 10, 10, 10 },
                new int[]{ 0, 0, 0 },
                new int[]{ 150, 100, 50 }
            )
        });

        List<List<INode>> nodes = new List<List<INode>>();
        nodes.AddRange(new List<List<INode>>() {
            Utilities.GenerateNodes(2, ComparisonType.Type.Equal),
            Utilities.GenerateNodes(2, ComparisonType.Type.Equal),
            Utilities.GenerateNodes(2, ComparisonType.Type.Equal),
            Utilities.GenerateNodes(2, ComparisonType.Type.More)
        });
        yield return new object[]
        {
            0,
            new int[] { 0, 1 },
            histograms[0],
            nodes[0]
        };

        yield return new object[]
        {
            1,
            new int[] { 0, 1 },
            histograms[1],
            nodes[1]
        };

        yield return new object[]
        {
            2,
            new int[] { 1, 0 },
            histograms[2],
            nodes[2]
        };

        yield return new object[]
        {
            3,
            new int[] { 1, 0 },
            histograms[3],
            nodes[3]
        };
    }


    [TestMethod]
    [DynamicData(nameof(OptimiseJoinQueryEqualData), DynamicDataSourceType.Method)]
    public void OptimiseJoinQueryEqual(int testID, int[] expectedOrder, List<HistogramEquiDepth> histograms, List<INode> nodes)
    {
        // Arrange
        var histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), 10);
        for (int i = 0; i < histograms.Count; i++)
            histogramManager.AddHistogram(histograms[i]);
        
        var queryOptimiser = new QueryOptimiserEquiDepth(histogramManager);

        // Act
        var optimiserResult = queryOptimiser.OptimiseQuery(nodes);

        // Assert
        for (int i = 0; i < expectedOrder.Length; i++) {
            Assert.AreEqual(expectedOrder[i], optimiserResult.EstCardinalities[i].Node.Id);
        }
    }
}