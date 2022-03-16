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
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID",
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID",
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 100 },
        new string[] { "A", "B", "C" })]
    [DataRow(
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID",
        "SELECT * FROM (B JOIN C ON B.ID = C.ID) JOIN A ON A.ID = B.ID",
        new int[] { 10, 0, 50 },
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 100 },
        new string[] { "A", "B", "C" })]
    [DataRow(
        "SELECT * FROM (Q JOIN B ON Q.ID = B.ID) JOIN C ON B.ID = C.ID",
        "SELECT * FROM (Q JOIN B ON Q.ID = B.ID) JOIN C ON B.ID = C.ID",
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 50 },
        new int[] { 10, 0, 100 },
        new string[] { "Q", "B", "C" })]
    [DataRow(
        "SELECT * FROM (D JOIN C ON D.ID = C.ID) JOIN A ON A.ID = D.ID",
        "SELECT * FROM (A JOIN D ON A.ID = D.ID) JOIN C ON D.ID = C.ID",
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 50 },
        new string[] { "A", "D", "C" })]
    public void OptimiseQueryString(string expected, string input, int[] aGramParam, int[] bGramParam, int [] cGramParam, string[] tableNames)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager("SomeConnectionString", 10);
        var aGram = Utilities.CreateIncreasingHistogram(tableNames[0], "ID", aGramParam[0], aGramParam[1], aGramParam[2]);
        var bGram = Utilities.CreateIncreasingHistogram(tableNames[1], "ID", bGramParam[0], bGramParam[1], bGramParam[2]);
        var cGram = Utilities.CreateIncreasingHistogram(tableNames[2], "ID", cGramParam[0], cGramParam[1], cGramParam[2]);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);
        histogramManager.AddHistogram(cGram);

        ParserManager PM = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
        List<INode> queryNodes = PM.ParseQuery(input);
        List<INode> expectedNodes = PM.ParseQuery(expected);

        var queryOptimiser = new QueryOptimiserEquiDepth(histogramManager);

        var resultNodes = queryOptimiser.OptimiseQuery(queryNodes);

        for (int i = 0; i < expectedNodes.Count; i++)
        {
            JoinNode expNode = expectedNodes[i] as JoinNode;
            JoinNode actNode = resultNodes[i].Node as JoinNode;

            Assert.AreEqual(expNode.JoinCondition, actNode.JoinCondition);
            Assert.AreEqual(expNode.ComType, actNode.ComType);
        }
    }
}