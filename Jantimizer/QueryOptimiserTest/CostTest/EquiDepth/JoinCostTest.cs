using DatabaseConnector;
using Histograms;
using Histograms.Models;
using Histograms.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Cost.Nodes.EquiDepth;
using QueryOptimiserTest;
using QueryParser;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using Tools.Models;

namespace CostEquiDepthTest;

[TestClass]
public class JoinCostTest
{
    #region Equality

    [TestMethod]
    [DataRow(10, 100, 100, 10000, 10)]
    [DataRow(20, 100, 100, 10000, 10)]
    [DataRow(10, 10, 100, 1000, 10)]
    [DataRow(10, 100, 100, 10000, 20)]
    [DataRow(100, 1, 1, 1, 10)]
    [DataRow("Jan", 100, 100, 10000, 10)]
    [DataRow("Jan", 100, 100, 10000, 10)]
    [DataRow("Jan", 10, 100, 1000, 10)]
    [DataRow("Jan", 100, 100, 10000, 20)]
    [DataRow("Jan", 1, 1, 1, 10)]
    public void EqualitySameValue(IComparable value, int aAmount, int bAmount, int expectedHits, int depth)
    {
        IHistogramManager<IHistogram, IDbConnector> histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateConstHistogram(Utilities.GetTableName(0), "ID", depth, aAmount, value);
        var bGram = Utilities.CreateConstHistogram(Utilities.GetTableName(1), "ID", depth, bAmount, value);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.Equal);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }

    [TestMethod]
    [DataRow(0, 100, 0, 100, 10, 10000)]
    [DataRow(0, 100, 99, 199, 10, 100)]
    [DataRow(0, 100, 101, 201, 10, 0)]
    [DataRow(0, 100, 50, 150, 10, 50 * 50)]
    [DataRow(50, 150, 0, 100, 10, 50 * 50)]
    [DataRow(0, 100, 50, 150, 20, 60 * 60)]
    public void EqualityOverlap(int aMin, int aMax, int bMin, int bMax, int depth, int expectedHits)
    {
        IHistogramManager<IHistogram, IDbConnector> histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(0), "ID", depth, aMin, aMax);
        var bGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(1), "ID", depth, bMin, bMax);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.Equal);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    #endregion
    #region Less
    [TestMethod]
    [DataRow(10, 20, 100, 100, 10, 100 * 100)]
    [DataRow(20, 20, 100, 100, 10, 0)]
    [DataRow(30, 20, 100, 100, 10, 0)]
    [DataRow(10, 20, 50, 100, 10, 50 * 100)]
    [DataRow(20, 20, 50, 100, 10, 0)]
    [DataRow(30, 20, 50, 100, 10, 0)]
    [DataRow(10, 20, 100, 50, 10, 50 * 100)]
    [DataRow(20, 20, 100, 50, 10, 0)]
    [DataRow(30, 20, 100, 50, 10, 0)]
    [DataRow(10, 20, 100, 100, 20, 100 * 100)]
    [DataRow(20, 20, 100, 100, 20, 0)]
    [DataRow(30, 20, 100, 100, 20, 0)]

    [DataRow("Jan", "Land!", 100, 100, 10, 100 * 100)]
    [DataRow("Jan", "And!", 100, 100, 10, 0)]
    [DataRow("Jan", "And!", 100, 100, 10, 0)]
    [DataRow("Jan", "Land!", 50, 100, 10, 50 * 100)]
    [DataRow("Jan", "And!", 50, 100, 10, 0)]
    [DataRow("Jan", "And!", 50, 100, 10, 0)]
    [DataRow("Jan", "Land!", 100, 50, 10, 50 * 100)]
    [DataRow("Jan", "And!", 100, 50, 10, 0)]
    [DataRow("Jan", "And!", 100, 50, 10, 0)]
    [DataRow("Jan", "Land!", 100, 100, 20, 100 * 100)]
    [DataRow("Jan", "And!", 100, 100, 20, 0)]
    [DataRow("Jan", "And!", 100, 100, 20, 0)]
    public void LessConstantValue(IComparable aValue, IComparable bValue, int aAmount, int bAmount, int depth, int expectedHits)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateConstHistogram(Utilities.GetTableName(0), "ID", depth, aAmount, aValue);
        var bGram = Utilities.CreateConstHistogram(Utilities.GetTableName(1), "ID", depth, bAmount, bValue);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.Less);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    [TestMethod]
    [DataRow(0, 100, 0, 100, 10, 100 * 100)]
    [DataRow(0, 100, 100, 200, 10, 100 * 100)]
    [DataRow(100, 200, 0, 100, 10, 0)]
    public void LessIncreasingValue(int aMin, int aMax, int bMin, int bMax, int depth, int expectedHits)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(0), "ID", depth, aMin, aMax);
        var bGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(1), "ID", depth, bMin, bMax);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.Less);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    #endregion
    #region More
    [TestMethod]
    [DataRow(10, 20, 100, 100, 10, 0)]
    [DataRow(20, 20, 100, 100, 10, 0)]
    [DataRow(30, 20, 100, 100, 10, 100 * 100)]
    [DataRow(10, 20, 50, 100, 10, 0)]
    [DataRow(20, 20, 50, 100, 10, 0)]
    [DataRow(30, 20, 50, 100, 10, 50 * 100)]
    [DataRow(10, 20, 100, 50, 10, 0)]
    [DataRow(20, 20, 100, 50, 10, 0)]
    [DataRow(30, 20, 100, 50, 10, 50 * 100)]
    [DataRow(10, 20, 100, 100, 20, 0)]
    [DataRow(20, 20, 100, 100, 20, 0)]
    [DataRow(30, 20, 100, 100, 20, 100 * 100)]

    [DataRow("Jan", "And!", 100, 100, 10, 100 * 100)]
    [DataRow("Jan", "Land!", 100, 100, 10, 0)]
    [DataRow("Jan", "Land!", 100, 100, 10, 0)]
    [DataRow("Jan", "And!", 50, 100, 10, 50 * 100)]
    [DataRow("Jan", "Land!", 50, 100, 10, 0)]
    [DataRow("Jan", "Land!", 50, 100, 10, 0)]
    [DataRow("Jan", "And!", 100, 50, 10, 50 * 100)]
    [DataRow("Jan", "Land!", 100, 50, 10, 0)]
    [DataRow("Jan", "Land!", 100, 50, 10, 0)]
    [DataRow("Jan", "And!", 100, 100, 20, 100 * 100)]
    [DataRow("Jan", "Land!", 100, 100, 20, 0)]
    [DataRow("Jan", "Land!", 100, 100, 20, 0)]
    public void MoreConstantValue(IComparable aValue, IComparable bValue, int aAmount, int bAmount, int depth, int expectedHits)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateConstHistogram(Utilities.GetTableName(0), "ID", depth, aAmount, aValue);
        var bGram = Utilities.CreateConstHistogram(Utilities.GetTableName(1), "ID", depth, bAmount, bValue);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.More);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    [TestMethod]
    [DataRow(0, 100, 0, 100, 10, 100 * 100)]
    [DataRow(0, 100, 100, 200, 10, 0)]
    [DataRow(100, 200, 0, 100, 10, 100 * 100)]
    public void MoreIncreasingValue(int aMin, int aMax, int bMin, int bMax, int depth, int expectedHits)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(0), "ID", depth, aMin, aMax);
        var bGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(1), "ID", depth, bMin, bMax);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.More);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    #endregion
    #region EqualOrLess
    [TestMethod]
    [DataRow(10, 20, 100, 100, 10, 100 * 100)]
    [DataRow(20, 20, 100, 100, 10, 100 * 100)]
    [DataRow(30, 20, 100, 100, 10, 0)]
    [DataRow(10, 20, 50, 100, 10, 50 * 100)]
    [DataRow(20, 20, 50, 100, 10, 50 * 100)]
    [DataRow(30, 20, 50, 100, 10, 0)]
    [DataRow(10, 20, 100, 50, 10, 50 * 100)]
    [DataRow(20, 20, 100, 50, 10, 50 * 100)]
    [DataRow(30, 20, 100, 50, 10, 0)]
    [DataRow(10, 20, 100, 100, 20, 100 * 100)]
    [DataRow(20, 20, 100, 100, 20, 100 * 100)]
    [DataRow(30, 20, 100, 100, 20, 0)]
    public void EqualOrLessConstantValue(int aValue, int bValue, int aAmount, int bAmount, int depth, int expectedHits)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateConstHistogram(Utilities.GetTableName(0), "ID", depth, aAmount, aValue);
        var bGram = Utilities.CreateConstHistogram(Utilities.GetTableName(1), "ID", depth, bAmount, bValue);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.EqualOrLess);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    [TestMethod]
    [DataRow(0, 100, 0, 100, 10, 100 * 100)]
    [DataRow(0, 100, 100, 200, 10, 100 * 100)]
    [DataRow(100, 200, 0, 100, 10, 0)]
    public void EqualOrLessIncreasingValue(int aMin, int aMax, int bMin, int bMax, int depth, int expectedHits)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(0), "ID", depth, aMin, aMax);
        var bGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(1), "ID", depth, bMin, bMax);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.EqualOrLess);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    #endregion
    #region EqualOrMore
    [TestMethod]
    [DataRow(10, 20, 100, 100, 10, 0)]
    [DataRow(20, 20, 100, 100, 10, 100 * 100)]
    [DataRow(30, 20, 100, 100, 10, 100 * 100)]
    [DataRow(10, 20, 50, 100, 10, 0)]
    [DataRow(20, 20, 50, 100, 10, 50 * 100)]
    [DataRow(30, 20, 50, 100, 10, 50 * 100)]
    [DataRow(10, 20, 100, 50, 10, 0)]
    [DataRow(20, 20, 100, 50, 10, 50 * 100)]
    [DataRow(30, 20, 100, 50, 10, 50 * 100)]
    [DataRow(10, 20, 100, 100, 20, 0)]
    [DataRow(20, 20, 100, 100, 20, 100 * 100)]
    [DataRow(30, 20, 100, 100, 20, 100 * 100)]
    public void EqualOrMoreConstantValue(int aValue, int bValue, int aAmount, int bAmount, int depth, int expectedHits)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateConstHistogram(Utilities.GetTableName(0), "ID", depth, aAmount, aValue);
        var bGram = Utilities.CreateConstHistogram(Utilities.GetTableName(1), "ID", depth, bAmount, bValue);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.EqualOrMore);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    [TestMethod]
    [DataRow(0, 100, 0, 100, 10, 100 * 100)]
    [DataRow(0, 100, 100, 200, 10, 0)]
    [DataRow(100, 200, 0, 100, 10, 100 * 100)]
    public void EqualOrMoreIncreasingValue(int aMin, int aMax, int bMin, int bMax, int depth, int expectedHits)
    {
        var histogramManager = new Histograms.Managers.PostgresEquiDepthHistogramManager(new ConnectionProperties(), depth);
        var aGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(0), "ID", depth, aMin, aMax);
        var bGram = Utilities.CreateIncreasingHistogram(Utilities.GetTableName(1), "ID", depth, bMin, bMax);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        var nodes = Utilities.GenerateNodes(1, ComparisonType.Type.EqualOrMore);

        var joinCost = new JoinCostEquiDepth();

        long cost = joinCost.CalculateCost((JoinNode)nodes[0], histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    #endregion
}