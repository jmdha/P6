using Histograms.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.QueryGenerators;
using QueryOptimiserTest;
using QueryParser;
using QueryParser.QueryParsers;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using QueryOptimiser.Cost.Nodes;

namespace GeneratorTest;

[TestClass]
public class PostgresGeneratorTest
{
    [TestMethod]
    [DataRow(
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID);", 
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID")]
    [DataRow(
        "SELECT * FROM (((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID) JOIN D ON C.ID = D.ID);", 
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID) JOIN D ON C.ID = D.ID")]
    public void JoinQueryUnvalued(string expected, string input)
    {
        ParserManager PM = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
        var nodes = PM.ParseQuery(input);

        PostgresGenerator gen = new PostgresGenerator();
        string query = gen.GenerateQuery(nodes);

        Assert.AreEqual(expected, query);
    }

    [TestMethod]
    [DataRow(
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID);", 
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID", 
        new []{ 0, 10 })]
    [DataRow(
        "SELECT * FROM ((A JOIN B ON B.ID = A.ID) JOIN C ON B.ID = C.ID);", 
        "SELECT * FROM (A JOIN B ON B.ID = A.ID) JOIN C ON B.ID = C.ID", 
        new []{ 0, 10 })]
    [DataRow(
        "SELECT * FROM ((A JOIN B ON B.ID = A.ID) JOIN C ON B.ID = C.ID);", 
        "SELECT * FROM (A JOIN B ON B.ID = A.ID) JOIN C ON B.ID = C.ID", 
        new []{ 0, 10 })]
    [DataRow(
        "SELECT * FROM ((B JOIN A ON A.ID = B.ID) JOIN C ON C.ID = B.ID);", 
        "SELECT * FROM (B JOIN A ON A.ID = B.ID) JOIN C ON C.ID = B.ID", 
        new []{ 0, 10 })]
    [DataRow(
        "SELECT * FROM ((B JOIN C ON B.ID = C.ID) JOIN A ON A.ID = B.ID);", 
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID", 
        new []{ 10, 0 })]
    [DataRow(
        "SELECT * FROM ((B JOIN C ON C.ID = B.ID) JOIN A ON A.ID = B.ID);", 
        "SELECT * FROM (A JOIN B ON A.ID = B.ID) JOIN C ON C.ID = B.ID", 
        new []{ 10, 0 })]
    [DataRow(
        "SELECT * FROM (((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID) JOIN D ON C.ID = D.ID);", 
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID) JOIN D ON C.ID = D.ID", 
        new []{ 0, 0, 0 })]
    [DataRow(
        "SELECT * FROM (((C JOIN D ON C.ID = D.ID) JOIN B ON B.ID = C.ID) JOIN A ON A.ID = B.ID);",
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID) JOIN D ON C.ID = D.ID",
        new[] { 10, 10, 0 })]
    [DataRow(
        "SELECT * FROM (((C JOIN D ON C.ID = D.ID) JOIN A ON A.ID = B.ID) JOIN B ON B.ID = C.ID);",
        "SELECT * FROM ((A JOIN B ON A.ID = B.ID) JOIN C ON B.ID = C.ID) JOIN D ON C.ID = D.ID",
        new[] { 10, 20, 0 })]
    public void JoinQueryValued(string expected, string input, int[] values)
    {
        ParserManager PM = new ParserManager(new List<IQueryParser>() { new JoinQueryParser() });
        var nodes = PM.ParseQuery(input);
        List<ValuedNode> valuedNodes = new List<ValuedNode>();
        for (int i = 0; i < nodes.Count; i++)
            valuedNodes.Add(new ValuedNode(values[i], nodes[i]));

        PostgresGenerator gen = new PostgresGenerator();
        string query = gen.GenerateQuery(valuedNodes);

        Assert.AreEqual(expected, query);
    }
}