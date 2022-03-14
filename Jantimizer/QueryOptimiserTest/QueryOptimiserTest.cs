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
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID);",
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID",
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 100 })]
    [DataRow(
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID);",
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID",
        new int[] { 10, 0, 50 },
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 100 })]
    [DataRow(
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID);",
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID",
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 50 },
        new int[] { 10, 0, 100 })]
    [DataRow(
        "SELECT * FROM ((B JOIN C ON B.ID = C.ID) JOIN A ON A.ID = B.ID);",
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID",
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 100 },
        new int[] { 10, 0, 50 })]

    public void OptimiseQueryString(string expected, string input, int[] aGramParam, int[] bGramParam, int [] cGramParam)
    {
        var histogramManager = new PostgresEquiDepthHistogramManager("SomeConnectionString", 10);
        var aGram = Utilities.CreateIncreasingHistogram("A", "ID", aGramParam[0], aGramParam[1], aGramParam[2]);
        var bGram = Utilities.CreateIncreasingHistogram("B", "ID", bGramParam[0], bGramParam[1], bGramParam[2]);
        var cGram = Utilities.CreateIncreasingHistogram("C", "ID", cGramParam[0], cGramParam[1], cGramParam[2]);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);
        histogramManager.AddHistogram(cGram);

        ParserManager PM = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
        var queryOptimiser = new QueryOptimiser.QueryOptimiser(PM, new QueryOptimiser.QueryGenerators.PostgresGenerator(), histogramManager);

        string query = queryOptimiser.OptimiseQuery(input);

        Assert.AreEqual(expected, query);
    }

}