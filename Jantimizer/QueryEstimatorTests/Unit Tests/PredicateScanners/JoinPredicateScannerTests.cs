using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryEstimator.Exceptions;
using QueryEstimator.Helpers;
using QueryEstimator.Models.PredicateScanners;
using QueryEstimator.PredicateScanners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Unit_Tests.PredicateScanners
{
    [TestClass]
    public class JoinPredicateScannerTests
    {
        #region Scan

        [TestMethod]
        public void Can_Scan_1()
        {
            // ARRANGE
            var tableAttr1 = new TableAttribute("A", "v");
            var tableAttr2 = new TableAttribute("B", "v");

            var opp1 = "=";
            var opp2 = "<";

            var constant = "50";

            IPredicateScanner<List<JoinNode>> scanner = new JoinPredicateScanner();
            var predicate1 = new JoinPredicate(
                new PredicateNode(tableAttr1),
                new PredicateNode(constant),
                opp1);
            var predicate2 = new JoinPredicate(
                new PredicateNode(tableAttr1),
                new PredicateNode(tableAttr2),
                opp2);
            var node = new JoinNode(new List<JoinPredicate>() { predicate1, predicate2 });
            var nodes = new List<JoinNode>() { node };

            // ACT
            scanner.Scan(nodes);

            // ASSERT
            var tableAttrPred = scanner.GetIfThere<TableAttributePredicate>()[0];
            Assert.AreEqual(tableAttr1, tableAttrPred.LeftTable);
            Assert.AreEqual(tableAttr2, tableAttrPred.RightTable);
            Assert.AreEqual(opp2, ComparisonType.GetOperatorString(tableAttrPred.ComType));
            var filterPred = scanner.GetIfThere<FilterPredicate>()[0];
            Assert.AreEqual(tableAttr1, filterPred.LeftTable);
            Assert.AreEqual(constant, filterPred.ConstantValue);
            Assert.AreEqual(opp1, ComparisonType.GetOperatorString(filterPred.ComType));
        }

        [TestMethod]
        public void Can_Scan_2()
        {
            // ARRANGE
            var tableAttr1 = new TableAttribute("A", "v");
            var tableAttr2 = new TableAttribute("B", "v");
            var tableAttr3 = new TableAttribute("B", "v");
            var tableAttr4 = new TableAttribute("B", "v");

            var opp1 = "=";
            var opp2 = "<";
            var opp3 = ">";

            IPredicateScanner<List<JoinNode>> scanner = new JoinPredicateScanner();
            var predicate1 = new JoinPredicate(
                new PredicateNode(tableAttr1),
                new PredicateNode(tableAttr2),
                opp1);
            var predicate2 = new JoinPredicate(
                new PredicateNode(tableAttr2),
                new PredicateNode(tableAttr3),
                opp2);
            var predicate3 = new JoinPredicate(
                new PredicateNode(tableAttr3),
                new PredicateNode(tableAttr4),
                opp3);
            var node = new JoinNode(new List<JoinPredicate>() { predicate1, predicate2, predicate3 });
            var nodes = new List<JoinNode>() { node };

            // ACT
            scanner.Scan(nodes);

            // ASSERT
            var tableAttrPred1 = scanner.GetIfThere<TableAttributePredicate>()[0];
            Assert.AreEqual(tableAttr1, tableAttrPred1.LeftTable);
            Assert.AreEqual(tableAttr2, tableAttrPred1.RightTable);
            Assert.AreEqual(opp1, ComparisonType.GetOperatorString(tableAttrPred1.ComType));

            var tableAttrPred2 = scanner.GetIfThere<TableAttributePredicate>()[1];
            Assert.AreEqual(tableAttr2, tableAttrPred2.LeftTable);
            Assert.AreEqual(tableAttr3, tableAttrPred2.RightTable);
            Assert.AreEqual(opp2, ComparisonType.GetOperatorString(tableAttrPred2.ComType));

            var tableAttrPred3 = scanner.GetIfThere<TableAttributePredicate>()[2];
            Assert.AreEqual(tableAttr3, tableAttrPred3.LeftTable);
            Assert.AreEqual(tableAttr4, tableAttrPred3.RightTable);
            Assert.AreEqual(opp3, ComparisonType.GetOperatorString(tableAttrPred3.ComType));
        }

        [TestMethod]
        public void Cant_Scan_IfFiltersOnly()
        {
            // ARRANGE
            IPredicateScanner<List<JoinNode>> scanner = new JoinPredicateScanner();
            var predicate1 = new JoinPredicate(
                new PredicateNode(new TableAttribute("A", "v")),
                new PredicateNode("50"),
                "=");
            var predicate2 = new JoinPredicate(
                new PredicateNode(new TableAttribute("B", "v")),
                new PredicateNode("50"),
                "<");
            var node = new JoinNode(new List<JoinPredicate>() { predicate1, predicate2 });
            var nodes = new List<JoinNode>() { node };

            // ACT
            try
            {
                scanner.Scan(nodes);
                Assert.Fail();
            }
            catch (PredicateScannerException ex)
            {
                Assert.AreEqual(PredicateScannerErrorType.NoTableAttributePrediacte, ex.Type);
            }
        }

        [TestMethod]
        public void Cant_Scan_IfFiltersNoTableAttributePredicate()
        {
            // ARRANGE
            IPredicateScanner<List<JoinNode>> scanner = new JoinPredicateScanner();
            var predicate1 = new JoinPredicate(
                new PredicateNode(new TableAttribute("A", "v")),
                new PredicateNode(new TableAttribute("B", "v")),
                "=");
            var predicate2 = new JoinPredicate(
                new PredicateNode(new TableAttribute("C", "v")),
                new PredicateNode("50"),
                "<");
            var node = new JoinNode(new List<JoinPredicate>() { predicate1, predicate2 });
            var nodes = new List<JoinNode>() { node };

            // ACT
            try
            {
                scanner.Scan(nodes);
                Assert.Fail();
            }
            catch (PredicateScannerException ex)
            {
                Assert.AreEqual(PredicateScannerErrorType.IlligalFilter, ex.Type);
            }
        }

        [TestMethod]
        public void Cant_Scan_IfInvalidPredicate()
        {
            // ARRANGE
            IPredicateScanner<List<JoinNode>> scanner = new JoinPredicateScanner();
            var predicate1 = new JoinPredicate(
                new PredicateNode(),
                new PredicateNode(),
                "=");
            var node = new JoinNode(new List<JoinPredicate>() { predicate1 });
            var nodes = new List<JoinNode>() { node };

            // ACT
            try
            {
                scanner.Scan(nodes);
                Assert.Fail();
            }
            catch (PredicateScannerException ex)
            {
                Assert.AreEqual(PredicateScannerErrorType.Unscannable, ex.Type);
            }
        }

        #endregion

        #region AddFilterIfValid

        [TestMethod]
        [DataRow("A", "v", "50", ComparisonType.Type.Equal)]
        [DataRow("A", "v", "10000", ComparisonType.Type.Less)]
        [DataRow("A", "v", "abc", ComparisonType.Type.EqualOrLess)]
        [DataRow("A", "v", "1513541", ComparisonType.Type.More)]
        [DataRow("A", "v", "0.5", ComparisonType.Type.EqualOrMore)]
        public void Can_AddFilterIfValid_CorrectWay(string tableName, string attrName, string constant, ComparisonType.Type comType)
        {
            // ARRANGE
            var tableAttr = new TableAttribute(tableName, attrName);
            JoinPredicateScanner scanner = new JoinPredicateScanner();
            JoinPredicate predicate = new JoinPredicate(
                new PredicateNode(tableAttr),
                new PredicateNode(constant),
                ComparisonType.GetOperatorString(comType));

            // ACT
            var wasSuccess = scanner.AddFilterIfValid(predicate);

            // ASSERT
            Assert.IsTrue(wasSuccess);
            var scanned = scanner._baseFilters[0];
            Assert.AreEqual(tableAttr, scanned.LeftTable);
            Assert.AreEqual(constant, scanned.ConstantValue);
            Assert.AreEqual(comType, scanned.ComType);
        }

        [TestMethod]
        [DataRow("A", "v", "50", ComparisonType.Type.Equal)]
        [DataRow("A", "v", "10000", ComparisonType.Type.Less)]
        [DataRow("A", "v", "abc", ComparisonType.Type.EqualOrLess)]
        [DataRow("A", "v", "1513541", ComparisonType.Type.More)]
        [DataRow("A", "v", "0.5", ComparisonType.Type.EqualOrMore)]
        public void Can_AddFilterIfValid_Invert(string tableName, string attrName, string constant, ComparisonType.Type comType)
        {
            // ARRANGE
            var tableAttr = new TableAttribute(tableName, attrName);
            JoinPredicateScanner scanner = new JoinPredicateScanner();
            JoinPredicate predicate = new JoinPredicate(
                new PredicateNode(constant),
                new PredicateNode(tableAttr),
                ComparisonType.GetOperatorString(comType));

            // ACT
            var wasSuccess = scanner.AddFilterIfValid(predicate);

            // ASSERT
            Assert.IsTrue(wasSuccess);
            var scanned = scanner._baseFilters[0];
            Assert.AreEqual(tableAttr, scanned.LeftTable);
            Assert.AreEqual(constant, scanned.ConstantValue);
            Assert.AreEqual(comType, ComparisonTypeHelper.InvertType(scanned.ComType));
        }

        [TestMethod]
        public void Cant_AddFilterIfValid_IfInvalid()
        {
            // ARRANGE
            var rightTableAttr = new TableAttribute("A", "v");
            var leftTableAttr = new TableAttribute("B", "v");
            JoinPredicateScanner scanner = new JoinPredicateScanner();
            JoinPredicate predicate = new JoinPredicate(
                new PredicateNode(leftTableAttr),
                new PredicateNode(rightTableAttr),
                "=");

            // ACT
            var wasSuccess = scanner.AddFilterIfValid(predicate);

            // ASSERT
            Assert.IsFalse(wasSuccess);
        }

        #endregion

        #region AddPredicateIfValid

        [TestMethod]
        [DataRow("A", "v", "B", "v" , ComparisonType.Type.Equal)]
        [DataRow("A", "v", "B", "v" , ComparisonType.Type.Less)]
        [DataRow("A", "v", "B", "v" , ComparisonType.Type.EqualOrLess)]
        [DataRow("A", "v", "B", "v" , ComparisonType.Type.More)]
        [DataRow("A", "v", "B", "v" , ComparisonType.Type.EqualOrMore)]
        public void Can_AddPredicateIfValid_First(string rightTableName, string rightAttrName, string leftTableName, string leftAttrName, ComparisonType.Type comType)
        {
            // ARRANGE
            var rightTableAttr = new TableAttribute(rightTableName, rightAttrName);
            var leftTableAttr = new TableAttribute(leftTableName, leftAttrName);
            JoinPredicateScanner scanner = new JoinPredicateScanner();
            JoinPredicate predicate = new JoinPredicate(
                new PredicateNode(leftTableAttr),
                new PredicateNode(rightTableAttr),
                ComparisonType.GetOperatorString(comType));

            // ACT
            var wasSuccess = scanner.AddPredicateIfValid(predicate);

            // ASSERT
            Assert.IsTrue(wasSuccess);
            var usedAttr = scanner._usedAttributes;
            var retPredicate = scanner.GetIfThere<TableAttributePredicate>()[0];
            Assert.AreEqual(leftTableAttr, retPredicate.LeftTable);
            Assert.AreEqual(rightTableAttr, retPredicate.RightTable);
            Assert.AreEqual(comType, retPredicate.ComType);
            Assert.AreEqual(leftTableAttr, usedAttr[0]);
            Assert.AreEqual(rightTableAttr, usedAttr[1]);
        }

        [TestMethod]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.Equal)]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.Less)]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.EqualOrLess)]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.More)]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.EqualOrMore)]
        public void Can_AddPredicateIfValid_SignificantFirst_Left(string rightTableName, string rightAttrName, string leftTableName, string leftAttrName, ComparisonType.Type comType)
        {
            // ARRANGE
            var rightTableAttr = new TableAttribute(rightTableName, rightAttrName);
            var leftTableAttr = new TableAttribute(leftTableName, leftAttrName);
            JoinPredicateScanner scanner = new JoinPredicateScanner();
            JoinPredicate predicate = new JoinPredicate(
                new PredicateNode(leftTableAttr),
                new PredicateNode(rightTableAttr),
                ComparisonType.GetOperatorString(comType));

            // ACT
            scanner._usedAttributes.Add(leftTableAttr);
            var wasSuccess = scanner.AddPredicateIfValid(predicate);

            // ASSERT
            Assert.IsTrue(wasSuccess);
            var usedAttr = scanner._usedAttributes;
            var retPredicate = scanner.GetIfThere<TableAttributePredicate>()[0];
            Assert.AreEqual(rightTableAttr, retPredicate.LeftTable);
            Assert.AreEqual(leftTableAttr, retPredicate.RightTable);
            Assert.AreEqual(ComparisonTypeHelper.InvertType(comType), retPredicate.ComType);
            Assert.AreEqual(leftTableAttr, usedAttr[0]);
            Assert.AreEqual(rightTableAttr, usedAttr[1]);
        }

        [TestMethod]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.Equal)]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.Less)]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.EqualOrLess)]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.More)]
        [DataRow("A", "v", "B", "v", ComparisonType.Type.EqualOrMore)]
        public void Can_AddPredicateIfValid_SignificantFirst_Right(string rightTableName, string rightAttrName, string leftTableName, string leftAttrName, ComparisonType.Type comType)
        {
            // ARRANGE
            var rightTableAttr = new TableAttribute(rightTableName, rightAttrName);
            var leftTableAttr = new TableAttribute(leftTableName, leftAttrName);
            JoinPredicateScanner scanner = new JoinPredicateScanner();
            JoinPredicate predicate = new JoinPredicate(
                new PredicateNode(leftTableAttr),
                new PredicateNode(rightTableAttr),
                ComparisonType.GetOperatorString(comType));

            // ACT
            scanner._usedAttributes.Add(rightTableAttr);
            var wasSuccess = scanner.AddPredicateIfValid(predicate);

            // ASSERT
            Assert.IsTrue(wasSuccess);
            var usedAttr = scanner._usedAttributes;
            var retPredicate = scanner.GetIfThere<TableAttributePredicate>()[0];
            Assert.AreEqual(leftTableAttr, retPredicate.LeftTable);
            Assert.AreEqual(rightTableAttr, retPredicate.RightTable);
            Assert.AreEqual(comType, retPredicate.ComType);
            Assert.AreEqual(rightTableAttr, usedAttr[0]);
            Assert.AreEqual(leftTableAttr, usedAttr[1]);
        }

        [TestMethod]
        public void Cant_AddPredicateIfValid_IfInvalid()
        {
            // ARRANGE
            var tableAttr = new TableAttribute("A", "v");
            JoinPredicateScanner scanner = new JoinPredicateScanner();
            JoinPredicate predicate = new JoinPredicate(
                new PredicateNode("a"),
                new PredicateNode(tableAttr),
                "=");

            // ACT
            var wasSuccess = scanner.AddPredicateIfValid(predicate);

            // ASSERT
            Assert.IsFalse(wasSuccess);
        }

        #endregion
    }
}
