using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class JoinNode : INode
    {
        public TableReferenceNode LeftTable { get; set; }
        public TableReferenceNode RightTable { get; set; }
        public List<JoinPredicate> Predicates { get; set; }

        public JoinNode(TableReferenceNode leftTable, TableReferenceNode rightTable, List<JoinPredicate> predicates)
        {
            LeftTable = leftTable;
            RightTable = rightTable;
            Predicates = predicates;
        }
        public JoinNode()
        {
            LeftTable = new TableReferenceNode();
            RightTable = new TableReferenceNode();
            Predicates = new List<JoinPredicate>();
        }

        public object Clone()
        {
            var newList = new List<JoinPredicate>();
            foreach(var pred in Predicates)
                if (pred.Clone() is JoinPredicate clone)
                    newList.Add(clone);
            if (LeftTable.Clone() is TableReferenceNode left)
                if (RightTable.Clone() is TableReferenceNode right)
                    return new JoinNode(left, right, newList);
            throw new ArgumentNullException("Could not clone");
        }

        public override bool Equals(object? obj)
        {
            return obj is JoinNode node &&
                   EqualityComparer<TableReferenceNode>.Default.Equals(LeftTable, node.LeftTable) &&
                   EqualityComparer<TableReferenceNode>.Default.Equals(RightTable, node.RightTable) &&
                   EqualityComparer<List<JoinPredicate>>.Default.Equals(Predicates, node.Predicates);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var pred in Predicates)
                hash += pred.GetHashCode();
            return hash + LeftTable.GetHashCode() + RightTable.GetHashCode();
        }
    }
}
