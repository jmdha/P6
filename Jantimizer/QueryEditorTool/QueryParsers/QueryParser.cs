using QueryEditorTool.Models;
using QueryEditorTool.Models.Constants;
using QueryEditorTool.Models.Expersions;
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
            INode? returnNode = ParseExpressions(null, query);

            return returnNode;
        }

        internal IExp? ParseExpressions(INode? parent, string subQuery)
        {
            subQuery = subQuery.Trim();
            if (subQuery.StartsWith("SELECT"))
            {
                var returnVal = new SELECTStmt(
                    parent,
                    ParseExpressions(null, subQuery.Substring(subQuery.IndexOf("FROM") + 5)),
                    null,
                    ParseConstant(null, subQuery.Substring(subQuery.IndexOf("SELECT") + 6, subQuery.IndexOf("FROM") - 6)));

                returnVal.Right.Parent = returnVal;
                returnVal.Value.Parent = returnVal;

                return returnVal;
            }
            else
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
                IExp? conditionExpression = ParseExpressions(null, binaryExp);

                var returnVal = new JOINStmt(
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
            else
            if (PredicateEnumParser.ContainsAny(subQuery) != "None")
            {
                string operatorValue = PredicateEnumParser.ContainsAny(subQuery);

                IConst? leftSide = ParseConstant(null, subQuery.Substring(0, subQuery.IndexOf(operatorValue)));
                IConst? rightSide = ParseConstant(null, subQuery.Substring(subQuery.IndexOf(operatorValue) + 2));

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
            return null;
        }

        internal IConst? ParseConstant(INode? parent, string subQuery)
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
            if (baseTree is JOINStmt join)
            {
                Console.WriteLine($"ID for join [{join}] is [{join.ItemIndex}]");
            }
            if (baseTree is IExp exp)
            {
                PrintJoinIDs(exp.Left);
                PrintJoinIDs(exp.Right);
                PrintJoinIDs(exp.Value);
            }
        }
    }
}
