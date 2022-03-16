using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class JoinPredicateRelation
    {
        public enum RelationType
        {
            None,
            And,
            Or,
            Predicate
        }

        public JoinNode.JoinPredicate? LeafPredicate { get; internal set; }
        public JoinPredicateRelation? LeftRelation { get; internal set; }
        public JoinPredicateRelation? RightRelation { get; internal set; }
        public RelationType Type { get; internal set; }

        public JoinPredicateRelation(JoinNode.JoinPredicate leafCondition)
        {
            LeafPredicate = leafCondition;
            Type = RelationType.Predicate;
        }

        public JoinPredicateRelation(JoinPredicateRelation leftRelation, JoinPredicateRelation rightRelation, RelationType type)
        {
            LeftRelation = leftRelation;
            RightRelation = rightRelation;
            Type = type;
        }

        public List<string> GetJoinTables(bool left = true, bool right = true)
        {
            List<string> tables = new List<string>();
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

        public static string GetRelationString(RelationType type)
        {
            switch (type)
            {
                case RelationType.None: throw new ArgumentException("Parameter type given was not initialized");
                case RelationType.And: return "AND";
                case RelationType.Or: return "OR";
                default:
                    throw new ArgumentException("Unhandled relation type", type.ToString());
            }
        }
    }
}
