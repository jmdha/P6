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
            INode? returnNode = ParseValueStatement(null, query);

            return returnNode;
        }

        internal IValue? ParseValueStatement(INode? parent, string subQuery)
        {
            subQuery = subQuery.Trim();
            if (subQuery.StartsWith("SELECT"))
            {
                var returnVal = new SELECTStmt(
                    parent,
                    ParseValueStatement(null, subQuery.Substring(subQuery.IndexOf("FROM") + 5)), 
                    null,
                    ParseConstant(null, subQuery.Substring(subQuery.IndexOf("SELECT") + 6, subQuery.IndexOf("FROM") - 6)));

                returnVal.Right.Parent = returnVal;
                returnVal.Value.Parent = returnVal;

                return returnVal;
            }
            if (subQuery.LastIndexOf("JOIN") != -1)
            {
                int joinIndex = subQuery.LastIndexOf("JOIN");

                string leftSide = subQuery.Substring(joinIndex + 4);
                string leftTableString = leftSide.Substring(0, leftSide.IndexOf("ON"));
                INode? leftTable = ParseValueStatement(null, leftTableString);
                if (leftTable == null)
                    leftTable = ParseConstant(null, leftTableString);

                string rigthSide = subQuery.Substring(0, joinIndex);
                INode? rightTable = ParseValueStatement(null, rigthSide);
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


                GetTablesUsedInConditionRec(returnVal.UsedTables, returnVal.Value);
                _movableIndex++;
                return returnVal;
            }
            return null;
        }

        internal IExp? ParseExpressions(INode? parent, string subQuery)
        {
            subQuery = subQuery.Trim();
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

        internal void GetTablesUsedInConditionRec(List<ConstVal> tables, INode parent)
        {
            if (parent is ConstVal tableValue)
            {
                if (tableValue.Value.Contains('.'))
                    tables.Add(new ConstVal(parent, tableValue.Value.Split('.')[0]));
            }
            if (parent is IExp exp)
            {
                GetTablesUsedInConditionRec(tables, exp.Right);
                GetTablesUsedInConditionRec(tables, exp.Left);
            }
            if (parent is IValue val)
            {
                GetTablesUsedInConditionRec(tables, val.Value);
            }
        }
    }
}
