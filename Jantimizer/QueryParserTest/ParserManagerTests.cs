using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryParser;
using QueryParser.Exceptions;
using QueryParser.Models;
using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParserTest
{
    [TestClass]
    public class ParserManagerTests
    {
        #region Constructor

        [TestMethod]
        public void Can_SetParsers()
        {
            // ARRANGE
            IQueryParser parser = new TestParser();

            // ACT
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ASSERT
            Assert.AreEqual(parser, newManager.QueryParsers[0]);
        }

        #endregion

        #region ParseQuery

        [TestMethod]
        public void Can_ParseQuery_IfParserSet()
        {
            // ARRANGE
            TestParser parser = new TestParser();
            parser.ShouldAccept = true;
            parser.ShouldReturn = new List<INode>() { new TestNode(0), new TestNode(1) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ACT
            var result = newManager.ParseQuery("anything");

            // ASSERT
            Assert.AreEqual(parser.ShouldReturn, result.Nodes);
        }

        [TestMethod]
        public void Can_ParseQuery_IfParsersSet()
        {
            // ARRANGE
            TestParser parser1 = new TestParser();
            TestParser parser2 = new TestParser();
            parser1.ShouldAccept = true;
            parser1.ShouldReturn = new List<INode>() { new TestNode(0), new TestNode(1) };
            parser2.ShouldAccept = false;
            parser2.ShouldReturn = new List<INode>() { new TestNode(2), new TestNode(3) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser2, parser1 });

            // ACT
            var result = newManager.ParseQuery("anything");

            // ASSERT
            Assert.AreEqual(parser1.ShouldReturn, result.Nodes);
        }

        [TestMethod]
        public void Cant_ParseQuery_IfParserNotSet_NoThrow()
        {
            // ARRANGE
            IParserManager newManager = new ParserManager(new List<IQueryParser>());

            // ACT
            var result = newManager.ParseQuery("anything", false);

            // ASSERT
            Assert.IsTrue(result.Nodes.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserErrorLogException))]
        public void Cant_ParseQuery_IfParserNotSet_Throw()
        {
            // ARRANGE
            IParserManager newManager = new ParserManager(new List<IQueryParser>());

            // ACT
            newManager.ParseQuery("anything", true);
        }

        #endregion

        #region ParseQueryAsync

        [TestMethod]
        public async Task Can_ParseQueryAsync_IfParserSet()
        {
            // ARRANGE
            TestParser parser = new TestParser();
            parser.ShouldAccept = true;
            parser.ShouldReturn = new List<INode>() { new TestNode(0), new TestNode(1) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ACT
            var result = await newManager.ParseQueryAsync("anything");

            // ASSERT
            Assert.AreEqual(parser.ShouldReturn, result.Nodes);
        }

        [TestMethod]
        public async Task Can_ParseQueryAsync_IfParsersSet()
        {
            // ARRANGE
            TestParser parser1 = new TestParser();
            TestParser parser2 = new TestParser();
            parser1.ShouldAccept = true;
            parser1.ShouldReturn = new List<INode>() { new TestNode(0), new TestNode(1) };
            parser2.ShouldAccept = false;
            parser2.ShouldReturn = new List<INode>() { new TestNode(2), new TestNode(3) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser2, parser1 });

            // ACT
            var result = await newManager.ParseQueryAsync("anything");

            // ASSERT
            Assert.AreEqual(parser1.ShouldReturn, result.Nodes);
        }

        [TestMethod]
        public async Task Cant_ParseQueryAsync_IfParserNotSet_NoThrow()
        {
            // ARRANGE
            IParserManager newManager = new ParserManager(new List<IQueryParser>());

            // ACT
            var result = await newManager.ParseQueryAsync("anything", false);

            // ASSERT
            Assert.IsTrue(result.Nodes.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ParserErrorLogException))]
        public async Task Cant_ParseQueryAsync_IfParserNotSet_Throw()
        {
            // ARRANGE
            IParserManager newManager = new ParserManager(new List<IQueryParser>());

            // ACT
            await newManager.ParseQueryAsync("anything", true);
        }

        #endregion

        #region ParseQuerySpecific

        [TestMethod]
        public void Can_ParseQuerySpecific()
        {
            // ARRANGE
            TestParser parser = new TestParser();
            parser.ShouldAccept = true;
            parser.ShouldReturn = new List<INode>() { new TestNode(0), new TestNode(1) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ACT
            var result = newManager.ParseQuerySpecific<TestParser>("anything", parser);

            // ASSERT
            Assert.AreEqual(parser.ShouldReturn, result.Nodes);
        }

        #endregion

        #region ParseQuerySpecificAsync

        [TestMethod]
        public async Task Can_ParseQuerySpecificAsync()
        {
            // ARRANGE
            TestParser parser = new TestParser();
            parser.ShouldAccept = true;
            parser.ShouldReturn = new List<INode>() { new TestNode(0), new TestNode(1) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ACT
            var result = await newManager.ParseQuerySpecificAsync<TestParser>("anything", parser);

            // ASSERT
            Assert.AreEqual(parser.ShouldReturn, result.Nodes);
        }

        #endregion
    }

    internal class TestParser : IQueryParser
    {
        public bool ShouldAccept = false;
        public List<INode> ShouldReturn = new List<INode>();

        public bool DoesQueryMatch(string query)
        {
            return ShouldAccept;
        }

        public Task<bool> DoesQueryMatchAsync(string query)
        {
            return Task.Run(() => ShouldAccept);
        }

        public List<INode> ParseQuery(string query)
        {
            return ShouldReturn;
        }

        public Task<List<INode>> ParseQueryAsync(string query)
        {
            return Task.Run(() => ShouldReturn);
        }
    }

    internal class TestNode : INode
    {
        public int Id { get; set; }

        public TestNode(int id)
        {
            Id = id;
        }
    }
}
