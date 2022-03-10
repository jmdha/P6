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



    // Both tables contain a 100 of the same value
    [TestMethod]
    public void SameConstant()
    {
        var histogramManager = new Histograms.Managers.PostgresEquiDepthHistogramManager("SomeConnectionString", 10);
        var aGram = CreateConstHistogram("A", "ID", 10, 100, 10);
        var bGram = CreateConstHistogram("B", "ID", 10, 100, 10);
        var cGram = CreateConstHistogram("C", "ID", 10, 100, 10);
        histogramManager.AddHistogram(aGram);
        histogramManager.AddHistogram(bGram);
        histogramManager.AddHistogram(cGram);

        QueryParser.ParserManager PM = new ParserManager(new List<IQueryParser>(){new JoinQueryParser()});
        var nodes = PM.ParseQuery("SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID");

        QueryHandler.QueryHandler QH = new QueryHandler.QueryHandler();
        int cost = QH.CalculateJoinCost(nodes[0] as QueryParser.Models.JoinNode, histogramManager);

        Assert.AreEqual(100 * 100, cost);
    }
}