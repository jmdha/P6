using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using QueryOptimiser;
using Histograms;
using Histograms.Managers;
using QueryParser;
using QueryParser.QueryParsers;
using QueryParser.Models;

namespace QueryOptimiserTest;

[TestClass]
public class QueryOptimiserTest
{
    [TestMethod]

    public void Temp()
    {
        var histogramManager = new PostgresEquiDepthHistogramManager("SomeConnectionString", 10);
        var aGram = Utilities.CreateIncreasingHistogram("A", "ID", 10, 0, 100);
        var bGram = Utilities.CreateIncreasingHistogram("B", "ID", 10, 50, 100);
        var cGram = Utilities.CreateIncreasingHistogram("C", "ID", 10, 50, 75);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);
        histogramManager.AddHistogram(cGram);

        ParserManager PM = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
        QueryOptimiser.QueryOptimiser queryOptimiser = new QueryOptimiser.QueryOptimiser(PM, histogramManager);

        var nodes = PM.ParseQuery("SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID");
        List<System.Tuple<INode, int>> valuedNodes = queryOptimiser.ValueQuery(nodes);


        Assert.AreEqual(1, 1);
    }

    public void Temp2()
    {
        var histogramManager = new PostgresEquiDepthHistogramManager("SomeConnectionString", 10);
        var aGram = Utilities.CreateIncreasingHistogram("A", "ID", 10, 0, 100);
        var bGram = Utilities.CreateIncreasingHistogram("B", "ID", 10, 50, 100);
        var cGram = Utilities.CreateIncreasingHistogram("C", "ID", 10, 50, 75);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);
        histogramManager.AddHistogram(cGram);

        ParserManager PM = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
        QueryOptimiser.QueryOptimiser queryOptimiser = new QueryOptimiser.QueryOptimiser(PM, histogramManager);

        var nodes = PM.ParseQuery("SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID");
        string query = queryOptimiser.OptimiseQuery(nodes);


        Assert.AreEqual(1, 1);
    }

}