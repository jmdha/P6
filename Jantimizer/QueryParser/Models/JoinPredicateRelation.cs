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

        public override int GetHashCode()
        {
            int hash = 0;
            if (LeafPredicate != null)
                hash += LeafPredicate.GetHashCode();
            if (LeftRelation != null)
                hash += LeftRelation.GetHashCode();
            if (RightRelation != null)
                hash += RightRelation.GetHashCode();
            if (Type != RelationType.Type.Predicate)
                return hash + HashCode.Combine(RelationType.GetRelationString(Type));
            else
                return hash + HashCode.Combine("Predicate");
        }
    }
}
