using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class FilterNode : INode
    {
        public TableReferenceNode TableReference { get; set; }
        public string Alias { get; set; }
        public string AttributeName { get; set; }
        public ComparisonType.Type ComType { get; set; }
        public IComparable Constant { get; set; }
        public int Id { get; private set; }

        public FilterNode(int id, TableReferenceNode reference, string alias, string attributeName, ComparisonType.Type comType, IComparable constant)
        {
            Id = id;
            TableReference = reference;
            Alias = alias;
            AttributeName = attributeName;
            ComType = comType;
            Constant = constant;
        }

        public override string ToString()
        {
            return $"{Alias}.{AttributeName} {ComType} {Constant}";
        }

        public override int GetHashCode()
        {
            return TableReference.GetHashCode() + HashCode.Combine(Id, Alias, AttributeName, ComparisonType.GetOperatorString(ComType), Constant);
        }

        public object Clone()
        {
            var newRefNode = TableReference.Clone();
            if (newRefNode != null)
                if (newRefNode is TableReferenceNode node)
                    return new FilterNode(Id, node, Alias, AttributeName, ComType, Constant);
            throw new InvalidDataException();
        }
    }
}
