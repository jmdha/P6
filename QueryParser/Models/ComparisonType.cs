using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class ComparisonType
    {
        public enum Type
        {
            None,
            EqualOrLess,
            EqualOrMore,
            Equal,
            Less,
            More
        };

        public static Type GetOperatorType(string stringOperator)
        {
            switch (stringOperator)
            {
                case "=": return Type.Equal;
                case "<": return Type.Less;
                case ">": return Type.More;
                case "<=": return Type.EqualOrLess;
                case ">=": return Type.EqualOrMore;
                default: throw new ArgumentOutOfRangeException("Unhandled join type " + nameof(stringOperator));
            }
        }
        public static string GetOperatorString(Type operatorType)
        {
            switch (operatorType)
            {
                case Type.Equal: return "=";
                case Type.Less: return "<";
                case Type.More: return ">";
                case Type.EqualOrLess: return "<=";
                case Type.EqualOrMore: return ">=";
                default: throw new ArgumentOutOfRangeException("Unhandled join string " + nameof(operatorType));
            }
        }
    }
}
