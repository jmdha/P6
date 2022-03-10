using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using QueryHandler;
using Histograms;
using Histograms.Managers;
using QueryParser;
using QueryParser.QueryParsers;



namespace QueryHandlerTest;

[TestClass]
public class SingleEqualityJoin
{
    Histograms.HistogramEquiDepth CreateConstHistogram(string tableName, string attibuteName, int depth, int tableSize, int value) {
        var tempGram = new Histograms.HistogramEquiDepth(tableName, attibuteName, depth);
        List<int> values = new List<int>();
        for (int i = 0; i < tableSize; i++)
            values.Add(value);
        tempGram.GenerateHistogram(values);
        return tempGram;
    }

    Histograms.HistogramEquiDepth CreateIncreasingHistogram(string tableName, string attibuteName, int depth, int minValue, int maxValue) {
        var tempGram = new Histograms.HistogramEquiDepth(tableName, attibuteName, depth);
        List<int> values = new List<int>();
        for (int i = minValue; i < maxValue; i++)
            values.Add(i);
        tempGram.GenerateHistogram(values);
        return tempGram;
    }



    [TestMethod]
    [DataRow(10, 100, 100, 10000, 10)]
    [DataRow(20, 100, 100, 10000, 10)]
    [DataRow(10, 10, 100, 1000, 10)]
    [DataRow(10, 100, 100, 10000, 20)]
    [DataRow(100, 1, 1, 1, 10)]
    public void SameValue(int value, int aAmount, int bAmount, int expectedHits, int depth)
    {
        var histogramManager = new Histograms.Managers.PostgresEquiDepthHistogramManager("SomeConnectionString", 10);
        var aGram = CreateConstHistogram("A", "ID", depth, aAmount, value);
        var bGram = CreateConstHistogram("B", "ID", depth, bAmount, value);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        QueryParser.ParserManager PM = new ParserManager(new List<IQueryParser>(){new JoinQueryParser()});
        var nodes = PM.ParseQuery("SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID");

        QueryHandler.QueryHandler QH = new QueryHandler.QueryHandler();
        int cost = QH.CalculateJoinCost(nodes[0] as QueryParser.Models.JoinNode, histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
    
    [TestMethod]
    [DataRow(0, 100, 0, 100, 10, 10000)]
    [DataRow(0, 100, 99, 199, 10, 100)]
    [DataRow(0, 100, 101, 201, 10, 0)]
    [DataRow(0, 100, 50, 150, 10, 50*50)]
    [DataRow(50, 150, 0, 100, 10, 50*50)]
    [DataRow(0, 100, 50, 150, 20, 60*60)]
    public void Overlap(int aMin, int aMax, int bMin, int bMax, int depth, int expectedHits) {
        var histogramManager = new Histograms.Managers.PostgresEquiDepthHistogramManager("SomeConnectionString", 10);
        var aGram = CreateIncreasingHistogram("A", "ID", depth, aMin, aMax);
        var bGram = CreateIncreasingHistogram("B", "ID", depth, bMin, bMax);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);

        QueryParser.ParserManager PM = new ParserManager(new List<IQueryParser>(){new JoinQueryParser()});
        var nodes = PM.ParseQuery("SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID");

        QueryHandler.QueryHandler QH = new QueryHandler.QueryHandler();
        int cost = QH.CalculateJoinCost(nodes[0] as QueryParser.Models.JoinNode, histogramManager);

        Assert.AreEqual(expectedHits, cost);
    }
}