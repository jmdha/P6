using QueryEditorTool.Models;
using QueryEditorTool.Models.Constants;
using QueryEditorTool.Models.Expersions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.QueryParsers
{
    public class QueryParser : IQueryParser
    {
        public INode? ParseQuery(string query)
        {
            INode? returnNode = null;
            query = query.ToUpper();
            returnNode = ParseStatement(query);

            return returnNode;
        }

        internal IStmt? ParseStatement(string subQuery)
        {
            subQuery = subQuery.Trim();
            if (subQuery.StartsWith("SELECT"))
            {
                string value = subQuery.Substring(subQuery.IndexOf("SELECT") + 6, subQuery.IndexOf("FROM") - 6);
                subQuery = subQuery.Substring(subQuery.IndexOf("FROM") + 5);
                IConst selectValue = new ConstVal(value);
                return new SELECTStmt(selectValue, ParseStatement(subQuery));
            }
            if (subQuery.LastIndexOf("JOIN") != -1)
            {
                int joinIndex = subQuery.LastIndexOf("JOIN");

                string leftSide = subQuery.Substring(joinIndex + 4);
                string leftTableString = leftSide.Substring(0, leftSide.IndexOf("ON"));
                INode leftTable = ParseConstant(leftTableString);
                if (leftTable == null)
                    leftTable = ParseStatement(leftTableString);

                string rigthSide = subQuery.Substring(0, joinIndex);
                INode rightTable = ParseConstant(rigthSide);
                if (rightTable == null)
                    rightTable = ParseStatement(rigthSide);

                string binaryExp = subQuery.Substring(subQuery.LastIndexOf("ON") + 2);
                IExp conditionExpression = ParseExpressions(binaryExp);

                return new JOINStmt(rightTable, leftTable, conditionExpression);
            }
            return null;
        }

        internal IExp? ParseExpressions(string subQuery)
        {
            subQuery = subQuery.Trim();
            if (PredicateEnumParser.ContainsAny(subQuery) != "None")
            {
                string operatorValue = PredicateEnumParser.ContainsAny(subQuery);
                string leftSideStr = subQuery.Substring(0, subQuery.IndexOf(operatorValue));
                IConst leftSide = ParseConstant(leftSideStr);

                string rightSideStr = subQuery.Substring(subQuery.IndexOf(operatorValue) + 2);
                IConst rightSide = ParseConstant(rightSideStr);

                return new BinaryExp(rightSide, PredicateEnumParser.ConvertToEnum(operatorValue), leftSide);

            }
            return null;
        }

        internal IConst? ParseConstant(string subQuery)
        {
            subQuery = subQuery.Trim();
            if ((subQuery.Count(x => x == '.')) == 1)
            {
                return new TableConst(subQuery.Substring(0, subQuery.IndexOf(".")), subQuery.Substring(subQuery.IndexOf(".") + 1));
            }
            if (!subQuery.Contains('(') && !subQuery.Contains(')'))
            {
                subQuery = subQuery.Trim(')');
                subQuery = subQuery.Trim('(');
                return new TableConst(subQuery);
            }
            return null;
        }
    }
}
