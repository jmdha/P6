using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class JoinPredicate : ICloneable
    {
        public PredicateNode LeftAttribute { get; set; }
        public PredicateNode RightAttribute { get; set; }
        public string ComType { get; set; }

        public JoinPredicate(PredicateNode leftAttribute, PredicateNode rightAttribute, string comType)
        {
            LeftAttribute = leftAttribute;
            RightAttribute = rightAttribute;
            ComType = comType;
        }

        public JoinPredicate()
        {
            LeftAttribute = new PredicateNode();
            RightAttribute = new PredicateNode();
            ComType = "";
        }

        public ComparisonType.Type GetComType()
        {
            return ComparisonType.GetOperatorType(ComType);
        }

        public override bool Equals(object? obj)
        {
            return obj is JoinPredicate predicate &&
                   EqualityComparer<PredicateNode>.Default.Equals(LeftAttribute, predicate.LeftAttribute) &&
                   EqualityComparer<PredicateNode>.Default.Equals(RightAttribute, predicate.RightAttribute) &&
                   ComType == predicate.ComType;
        }

        public override int GetHashCode()
        {
            return LeftAttribute.GetHashCode() + RightAttribute.GetHashCode() + HashCode.Combine(ComType);
        }

        public object Clone()
        {
            if (LeftAttribute.Clone() is PredicateNode left)
                if (RightAttribute.Clone() is PredicateNode right)
                    return new JoinPredicate(left, right, ComType);
            throw new ArgumentNullException("Could not clone");
        }

        public override string? ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Left Attribute: {LeftAttribute}");
            sb.AppendLine($"Right Attribute: {RightAttribute}");
            sb.AppendLine($"Comparison Type: {ComType}");
            return sb.ToString();
        }
    }
}
