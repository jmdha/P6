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
        internal static (JoinNode.ComparisonType, string)[] Operators = new (JoinNode.ComparisonType, string)[]{
                        (JoinNode.ComparisonType.EqualOrLess, "<="),
                        (JoinNode.ComparisonType.EqualOrMore, ">="),
                        (JoinNode.ComparisonType.Equal, "="),
                        (JoinNode.ComparisonType.More, ">"),
                        (JoinNode.ComparisonType.Less, "<")
                    };
}
}
