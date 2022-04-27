using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class JoinNode : INode
    {
        public List<TableReferenceNode> Tables { get {
                var newList = new List<TableReferenceNode>();
                foreach (var predicate in Predicates)
                {
                    if (predicate.LeftAttribute.Attribute != null)
                        newList.Add(predicate.LeftAttribute.Attribute.Table);
                    if (predicate.RightAttribute.Attribute != null)
                        newList.Add(predicate.RightAttribute.Attribute.Table);
                }
                return newList;
            } 
        }
        public List<JoinPredicate> Predicates { get; set; }

        public JoinNode(List<JoinPredicate> predicates)
        {
            Predicates = predicates;
        }

        public JoinNode()
        {
            Predicates = new List<JoinPredicate>();
        }

        public object Clone()
        {
            var newList = new List<JoinPredicate>();
            foreach(var pred in Predicates)
                if (pred.Clone() is JoinPredicate clone)
                    newList.Add(clone);
            return new JoinNode(newList);
        }

        public override bool Equals(object? obj)
        {
            return obj is JoinNode node &&
                   EqualityComparer<List<TableReferenceNode>>.Default.Equals(Tables, node.Tables) &&
                   EqualityComparer<List<JoinPredicate>>.Default.Equals(Predicates, node.Predicates);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var pred in Predicates)
                hash += pred.GetHashCode();
            foreach (var table in Tables)
                hash += table.GetHashCode();
            return hash;
        }
    }
}
