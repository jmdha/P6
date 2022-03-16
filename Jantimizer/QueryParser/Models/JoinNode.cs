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

        public JoinNode(int id, string predicate)
        {
            Id = id;
            Predicate = predicate;
            Relation = ExtrapolateRelation(predicate);
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

        private static JoinPredicateRelation ExtrapolateRelation(string predicate)
        {
            JoinPredicateRelation.RelationType[] relationTypes = new JoinPredicateRelation.RelationType[] { JoinPredicateRelation.RelationType.And, JoinPredicateRelation.RelationType.Or };
            string[] sides = new string[] {};
            JoinPredicateRelation.RelationType relationType = JoinPredicateRelation.RelationType.None;
            for (int i = 0; i < relationTypes.Length; i++)
            {
                sides = predicate.Split(JoinPredicateRelation.GetRelationString(relationTypes[i]));
                if (sides.Length == 2)
                {
                    relationType = relationTypes[i];
                    break;
                }
            }
            if (relationType == JoinPredicateRelation.RelationType.None || sides.Length < 1)
                return new JoinPredicateRelation(ExtrapolateJoinPredicate(predicate.Replace("(", "").Replace(")", "")));
            else if (sides.Length != 2)
                throw new InvalidDataException("Somehow only had one side " + predicate);

            JoinPredicateRelation leftRelation = ExtrapolateRelation(sides[0]);
            JoinPredicateRelation rightRelation = ExtrapolateRelation(sides[1]);

            return new JoinPredicateRelation(leftRelation, rightRelation, relationType);
        }

        private static JoinPredicate ExtrapolateJoinPredicate(string predicate)
        {
            var operatorTypes = (ComparisonType.Type[])Enum.GetValues(typeof(ComparisonType.Type));
            string[] predicateSplit = new string[] {};
            ComparisonType.Type comparisonType = ComparisonType.Type.None;
            foreach (var op in operatorTypes)
            {
                if (op == ComparisonType.Type.None)
                    continue;
                string operatorString = ComparisonType.GetOperatorString(op);
                if (predicate.Contains(operatorString))
                {
                    predicateSplit = predicate.Split($" {operatorString} ");
                    comparisonType = op;
                    break;
                }
            }
            if (comparisonType == ComparisonType.Type.None)
                throw new InvalidDataException("Has no operator " + predicate);

            string[] leftSplit = predicateSplit[0].Split(".");
            string[] rightSplit = predicateSplit[1].Split(".");

            if (leftSplit.Length != 2 || rightSplit.Length != 2)
                throw new InvalidDataException("Invalid split " + predicateSplit[0] + " " + predicateSplit[1]);

            return new JoinPredicate(
                leftSplit[0],
                leftSplit[1],
                rightSplit[0],
                rightSplit[1],
                predicate,
                comparisonType
                );
        }
    }
}
