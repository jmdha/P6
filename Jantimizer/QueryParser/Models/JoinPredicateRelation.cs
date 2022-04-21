using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class JoinPredicateRelation
    {
        public JoinPredicate? LeafPredicate { get; internal set; }
        public JoinPredicateRelation? LeftRelation { get; internal set; }
        public JoinPredicateRelation? RightRelation { get; internal set; }
        public RelationType.Type Type { get; internal set; }

        public JoinPredicateRelation(JoinPredicate leafCondition)
        {
            LeafPredicate = leafCondition;
            Type = RelationType.Type.Predicate;
        }

        public JoinPredicateRelation(JoinPredicateRelation? leftRelation, JoinPredicateRelation? rightRelation, RelationType.Type type)
        {
            LeftRelation = leftRelation;
            RightRelation = rightRelation;
            Type = type;
        }

        public List<TableReferenceNode> GetJoinTables(bool left = true, bool right = true)
        {
            List<TableReferenceNode> tables = new List<TableReferenceNode>();
            if (LeftRelation != null)
                tables.AddRange(LeftRelation.GetJoinTables());
            if (RightRelation != null)
                tables.AddRange(RightRelation.GetJoinTables());
            if (LeafPredicate != null)
            {
                if (left)
                    tables.Add(LeafPredicate.LeftTable);
                if (right)
                    tables.Add(LeafPredicate.RightTable);
            }
            return tables;
        }

        public override bool Equals(object? obj)
        {
            return obj is JoinPredicateRelation relation &&
                   EqualityComparer<JoinPredicate?>.Default.Equals(LeafPredicate, relation.LeafPredicate) &&
                   EqualityComparer<JoinPredicateRelation?>.Default.Equals(LeftRelation, relation.LeftRelation) &&
                   EqualityComparer<JoinPredicateRelation?>.Default.Equals(RightRelation, relation.RightRelation) &&
                   Type == relation.Type;
        }

        public override int GetHashCode()
        {
            var hash = 0;
            if (LeafPredicate != null)
                hash += LeafPredicate.GetHashCode();
            if (LeftRelation != null)
                hash += LeftRelation.GetHashCode();
            if (RightRelation != null)
                hash += RightRelation.GetHashCode();
            return hash + Type.GetHashCode();
        }
    }
}
