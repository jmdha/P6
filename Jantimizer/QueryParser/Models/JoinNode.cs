using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public partial class JoinNode : INode
    {
        public int Id { get; internal set; }
        public string Predicate { get; internal set; }
        public JoinPredicateRelation Relation { get; internal set; }

        public JoinNode(int id, string predicate, JoinPredicateRelation relation)
        {
            Id = id;
            Predicate = predicate;
            Relation = relation;
        }

        public JoinNode(int id, string predicate, ComparisonType.Type type, string leftTable, string leftAttribute, string rightTable, string rightAttribute)
        {
            Id = id;
            Predicate = predicate;
            Relation = new JoinPredicateRelation(new JoinPredicate(
                leftTable,
                leftAttribute,
                rightTable,
                rightAttribute,
                predicate,
                type));
        }

        public override string? ToString()
        {
            if (Relation.LeafPredicate != null)
            {
                if (Relation.LeafPredicate.LeftTable.CompareTo(Relation.LeafPredicate.RightTable) <= 0)
                    return $"({Relation.LeafPredicate.LeftTable} JOIN {Relation.LeafPredicate.RightTable} ON {Predicate})";
                else
                    return $"({Relation.LeafPredicate.RightTable} JOIN {Relation.LeafPredicate.LeftTable} ON {Predicate})";

            }
                
            return $"({Predicate})";
        }

        // The table which should be joined on
        public string GetSuffixString(string param)
        {
            return $" JOIN {param} ON {Predicate})";
        }
    }
}
