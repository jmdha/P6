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
            throw new NotImplementedException();
        }
    }
}
