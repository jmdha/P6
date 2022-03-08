using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models.Expersions
{
    public class BinaryExp : IExp
    {
        public IConst? Right { get; set; }
        public PredicateEnums Predicate { get; set; }
        public IConst? Left { get; set; }

        public BinaryExp(IConst? right, PredicateEnums predicate, IConst? left)
        {
            Right = right;
            Predicate = predicate;
            Left = left;
        }

        public override string? ToString()
        {
            return $"{Left} {PredicateEnumParser.ConvertToString(Predicate)} {Right}";
        }
    }
}
