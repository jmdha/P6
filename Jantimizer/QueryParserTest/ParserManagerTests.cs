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
            parser.ShouldReturnJoins = new List<JoinNode>() { new TestJoinNode(0), new TestJoinNode(1) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ACT
            var result = newManager.ParseQuery("anything");

            // ASSERT
            Assert.AreEqual(parser.ShouldReturnJoins, result.Joins);
        }

        [TestMethod]
        public void Can_ParseQuery_IfParsersSet()
        {
            // ARRANGE
            TestParser parser1 = new TestParser();
            TestParser parser2 = new TestParser();
            parser1.ShouldAccept = true;
            parser1.ShouldReturnJoins = new List<JoinNode>() { new TestJoinNode(0), new TestJoinNode(1) };
            parser2.ShouldAccept = false;
            parser2.ShouldReturnJoins = new List<JoinNode>() { new TestJoinNode(2), new TestJoinNode(3) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser2, parser1 });

            // ACT
            var result = newManager.ParseQuery("anything");

            // ASSERT
            Assert.AreEqual(parser1.ShouldReturnJoins, result.Joins);
        }

        [TestMethod]
        public void Cant_ParseQuery_IfParserNotSet_NoThrow()
        {
            // ARRANGE
            IParserManager newManager = new ParserManager(new List<IQueryParser>());

            // ACT
            var result = newManager.ParseQuery("anything", false);

            // ASSERT
            Assert.IsTrue(result.Joins.Count == 0);
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
            parser.ShouldReturnJoins = new List<JoinNode>() { new TestJoinNode(0), new TestJoinNode(1) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ACT
            var result = await newManager.ParseQueryAsync("anything");

            // ASSERT
            Assert.AreEqual(parser.ShouldReturnJoins, result.Joins);
        }

        [TestMethod]
        public async Task Can_ParseQueryAsync_IfParsersSet()
        {
            // ARRANGE
            TestParser parser1 = new TestParser();
            TestParser parser2 = new TestParser();
            parser1.ShouldAccept = true;
            parser1.ShouldReturnJoins = new List<JoinNode>() { new TestJoinNode(0), new TestJoinNode(1) };
            parser2.ShouldAccept = false;
            parser2.ShouldReturnJoins = new List<JoinNode>() { new TestJoinNode(2), new TestJoinNode(3) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser2, parser1 });

            // ACT
            var result = await newManager.ParseQueryAsync("anything");

            // ASSERT
            Assert.AreEqual(parser1.ShouldReturnJoins, result.Joins);
        }

        [TestMethod]
        public async Task Cant_ParseQueryAsync_IfParserNotSet_NoThrow()
        {
            // ARRANGE
            IParserManager newManager = new ParserManager(new List<IQueryParser>());

            // ACT
            var result = await newManager.ParseQueryAsync("anything", false);

            // ASSERT
            Assert.IsTrue(result.Joins.Count == 0);
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
            parser.ShouldReturnJoins = new List<JoinNode>() { new TestJoinNode(0), new TestJoinNode(1) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ACT
            var result = newManager.ParseQuerySpecific<TestParser>("anything", parser);

            // ASSERT
            Assert.AreEqual(parser.ShouldReturnJoins, result.Joins);
        }

        #endregion

        #region ParseQuerySpecificAsync

        [TestMethod]
        public async Task Can_ParseQuerySpecificAsync()
        {
            // ARRANGE
            TestParser parser = new TestParser();
            parser.ShouldAccept = true;
            parser.ShouldReturnJoins = new List<JoinNode>() { new TestJoinNode(0), new TestJoinNode(1) };
            IParserManager newManager = new ParserManager(new List<IQueryParser>() { parser });

            // ACT
            var result = await newManager.ParseQuerySpecificAsync<TestParser>("anything", parser);

            // ASSERT
            Assert.AreEqual(parser.ShouldReturnJoins, result.Joins);
        }

        #endregion
    }

    internal class TestParser : IQueryParser
    {
        public bool ShouldAccept = false;
        public List<JoinNode> ShouldReturnJoins = new List<JoinNode>();
        public List<FilterNode> ShouldReturnFilters = new List<FilterNode>();

        public bool DoesQueryMatch(string query)
        {
            return ShouldAccept;
        }

        public Task<bool> DoesQueryMatchAsync(string query)
        {
            return Task.Run(() => ShouldAccept);
        }

        public ParserResult ParseQuery(string query)
        {
            return new ParserResult(ShouldReturnJoins, ShouldReturnFilters, new Dictionary<string, TableReferenceNode>(), query);
        }

        public Task<ParserResult> ParseQueryAsync(string query)
        {
            return Task.Run(() => new ParserResult(ShouldReturnJoins, ShouldReturnFilters, new Dictionary<string, TableReferenceNode>(), query));
        }
    }

    internal class TestJoinNode : JoinNode
    {
        public TestJoinNode(int id) : base(id, "", new JoinPredicateRelation(null, null, RelationType.Type.None))
        {
        }
    }
}
