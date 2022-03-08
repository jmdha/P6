using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    public enum PredicateEnums
    {
        None,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Equal
    }

    public static class PredicateEnumParser
    {
        public static string ConvertToString(PredicateEnums value)
        {
            switch (value)
            {
                case PredicateEnums.None: return "None";
                case PredicateEnums.GreaterThan: return ">";
                case PredicateEnums.GreaterThanOrEqual: return ">=";
                case PredicateEnums.LessThan: return "<";
                case PredicateEnums.LessThanOrEqual: return "<=";
                case PredicateEnums.Equal: return "=";
            }
            return "None";
        }

        public static PredicateEnums ConvertToEnum(string value)
        {
            switch (value)
            {
                case "None": return PredicateEnums.None;
                case ">": return PredicateEnums.GreaterThan;
                case ">=": return PredicateEnums.GreaterThanOrEqual;
                case "<": return PredicateEnums.LessThan;
                case "<=": return PredicateEnums.LessThanOrEqual;
                case "=": return PredicateEnums.Equal;
            }
            return PredicateEnums.None;
        }

        public static string ContainsAny(string value)
        {
            foreach(var enumValue in (PredicateEnums[]) Enum.GetValues(typeof(PredicateEnums)))
            {
                string strValue = ConvertToString(enumValue);
                if (value.Contains(strValue))
                    return strValue;
            }
            return "None";
        }
    }
}
