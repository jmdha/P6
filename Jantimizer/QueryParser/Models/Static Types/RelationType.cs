using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public static class RelationType
    {
        public enum Type
        {
            None,
            And,
            Or,
            Predicate
        }

        public static Type GetRelationType(string stringType)
        {
            switch (stringType.ToUpper())
            {
                case "AND": return Type.And;
                case "OR": return Type.Or;
                default : return Type.Predicate;
            }
        }

        public static string GetRelationString(Type type)
        {
            switch (type)
            {
                case Type.None: throw new ArgumentException("Parameter type given was not initialized");
                case Type.And: return "AND";
                case Type.Or: return "OR";
                default:
                    throw new ArgumentException("Unhandled relation type", type.ToString());
            }
        }
    }
}
