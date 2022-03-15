using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.QueryParsers
{
    internal static class Utilities
    {
        internal static JoinNode.ComparisonType GetOperatorType(string stringOperator)
        {
            switch (stringOperator)
            {
                case "=": return JoinNode.ComparisonType.Equal;
                case "<": return JoinNode.ComparisonType.Less;
                case ">": return JoinNode.ComparisonType.More;
                case "<=": return JoinNode.ComparisonType.EqualOrLess;
                case ">=": return JoinNode.ComparisonType.EqualOrMore;
                default: throw new ArgumentOutOfRangeException("Unhandled join type " + nameof(stringOperator));
            }
        }
        internal static string GetOperatorString(JoinNode.ComparisonType operatorType)
        {
            switch (operatorType)
            {
                case JoinNode.ComparisonType.Equal: return "=";
                case JoinNode.ComparisonType.Less: return "<";
                case JoinNode.ComparisonType.More: return ">";
                case JoinNode.ComparisonType.EqualOrLess: return "<=";
                case JoinNode.ComparisonType.EqualOrMore: return ">=";
                default: throw new ArgumentOutOfRangeException("Unhandled join string " + nameof(operatorType));
            }
        }
    }
}
