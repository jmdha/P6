using QueryEditorTool.Models;
using QueryEditorTool.Models.Constants;
using QueryEditorTool.Models.Expersions;
using QueryEditorTool.Models.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.QueryParsers
{
    public class QueryParser : IQueryParser
    {
        private int _movableIndex = 0;

        public INode? ParseQuery(string query)
        {
            query = query.ToUpper();
            INode? returnNode = ParseStatement(null, query);

            return returnNode;
        }

        internal INode? ParseStatement(INode? parent, string subQuery)
        {
            subQuery = subQuery.Trim();
            if (subQuery.StartsWith("SELECT"))
            {
                var returnVal = new SELECTStmt(
                    parent,
                    ParseStatement(null, subQuery.Substring(subQuery.IndexOf("FROM") + 5)),
                    ParseConstant(null, subQuery.Substring(subQuery.IndexOf("SELECT") + 6, subQuery.IndexOf("FROM") - 6)));

                returnVal.Child.Parent = returnVal;
                returnVal.Value.Parent = returnVal;

                return returnVal;
            }
            else if (subQuery.StartsWith("WHERE"))
            {
                var returnVal = new WHEREStmt(
                    parent,
                    ParseStatement(null, subQuery.Substring(subQuery.IndexOf("WHERE") + 5)));

                returnVal.Child.Parent = returnVal;

                return returnVal;
            }
            return ParseExpressions(parent, subQuery);
        }

        internal INode? ParseExpressions(INode? parent, string subQuery)
        {
            subQuery = subQuery.Trim();
            if (subQuery.LastIndexOf("JOIN") != -1)
            {
                int joinIndex = subQuery.LastIndexOf("JOIN");

                string leftSide = subQuery.Substring(joinIndex + 4);
                string leftTableString = leftSide.Substring(0, leftSide.IndexOf("ON"));
                INode? leftTable = ParseExpressions(null, leftTableString);
                if (leftTable == null)
                    leftTable = ParseConstant(null, leftTableString);

                string rigthSide = subQuery.Substring(0, joinIndex);
                INode? rightTable = ParseExpressions(null, rigthSide);
                if (rightTable == null)
                    rightTable = ParseConstant(null, rigthSide);

                string binaryExp = subQuery.Substring(subQuery.LastIndexOf("ON") + 2);
                INode? conditionExpression = ParseExpressions(null, binaryExp);

                var returnVal = new JOINExpr(
                    parent,
                    leftTable,
                    rightTable,
                    conditionExpression,
                    _movableIndex);

                returnVal.Left.Parent = returnVal;
                returnVal.Right.Parent = returnVal;
                returnVal.Value.Parent = returnVal;


                returnVal.GetTablesUsedInConditionRec(returnVal.UsedTables, returnVal.Value);
                _movableIndex++;
                return returnVal;
            }
            else if (PredicateEnumParser.ContainsAny(subQuery) != "None")
            {
                string operatorValue = PredicateEnumParser.ContainsAny(subQuery);

                INode? leftSide = ParseExpressions(null, subQuery.Substring(0, subQuery.IndexOf(operatorValue)));
                INode? rightSide = ParseExpressions(null, subQuery.Substring(subQuery.IndexOf(operatorValue) + 2));

                var returnVal = new BinaryExp(
                    parent,
                    rightSide, 
                    new ConstVal(null, $" {operatorValue} "),
                    leftSide);

                returnVal.Left.Parent = returnVal;
                returnVal.Right.Parent = returnVal;
                returnVal.Value.Parent = returnVal;

                return returnVal;
            }
            else
            if ((subQuery.Count(x => x == '.')) == 1)
            {
                var returnVal = new BinaryExp(
                    parent,
                    ParseConstant(null, subQuery.Substring(0, subQuery.IndexOf("."))),
                    new ConstVal(null, "."),
                    ParseConstant(null, subQuery.Substring(subQuery.IndexOf(".") + 1)));

                returnVal.Left.Parent = returnVal;
                returnVal.Right.Parent = returnVal;
                returnVal.Value.Parent = returnVal;

                return returnVal;
            }
            return ParseConstant(parent, subQuery);
        }

        internal INode? ParseConstant(INode? parent, string subQuery)
        {
            subQuery = subQuery.Trim();
            subQuery = subQuery.Trim(')');
            subQuery = subQuery.Trim('(');
            if (!subQuery.Contains('(') && !subQuery.Contains(')'))
            {
                return new ConstVal(parent, subQuery);
            }
            return null;
        }

        public void PrintJoinIDs(INode baseTree)
        {
            if (baseTree is JOINExpr join)
            {
                Console.WriteLine($"ID for join [{join}] is [{join.ItemIndex}]");
            }
            if (baseTree is IExp exp)
            {
                PrintJoinIDs(exp.Left);
                PrintJoinIDs(exp.Right);
                PrintJoinIDs(exp.Value);
            }
            if (baseTree is IStmt stmt)
            {
                PrintJoinIDs(stmt.Child);
            }
        }
    }
}
