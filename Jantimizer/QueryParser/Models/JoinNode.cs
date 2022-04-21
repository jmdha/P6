using QueryParser.QueryParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class JoinNode : INode
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

        public JoinNode(int id, string predicate, ComparisonType.Type type, TableReferenceNode leftTable, string leftAttribute, TableReferenceNode rightTable, string rightAttribute)
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
                if (Relation.LeafPredicate.LeftTable.Alias.CompareTo(Relation.LeafPredicate.RightTable.Alias) <= 0)
                    return $"({Relation.LeafPredicate.LeftTable} JOIN {Relation.LeafPredicate.RightTable} ON {Predicate})";
                else
                    return $"({Relation.LeafPredicate.RightTable} JOIN {Relation.LeafPredicate.LeftTable} ON {Predicate})";

            }
                
            return $"({Predicate})";
        }

        public override bool Equals(object? obj)
        {
            return obj is JoinNode node &&
                   Id == node.Id &&
                   Predicate == node.Predicate &&
                   EqualityComparer<JoinPredicateRelation>.Default.Equals(Relation, node.Relation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToString());
        }
    }
}
