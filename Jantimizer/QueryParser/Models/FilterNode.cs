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
        public string AttributeName { get; set; }
        public ComparisonType.Type ComType { get; set; }
        public IComparable Constant { get; set; }
        public int Id { get; private set; }

        public FilterNode(int id, TableReferenceNode tableReference, string attributeName, ComparisonType.Type comType, IComparable constant)
        {
            Id = id;
            TableReference = tableReference;
            AttributeName = attributeName;
            ComType = comType;
            Constant = constant;
        }

        public override string ToString()
        {
            return $"{TableReference.Alias}.{AttributeName} {ComType} {Constant}";
        }

        public override int GetHashCode()
        {
            return TableReference.GetHashCode() + HashCode.Combine(AttributeName, ComparisonType.GetOperatorString(ComType), Constant);
        }

        public object Clone()
        {
            var newReference = TableReference.Clone();
            if (newReference is TableReferenceNode refNode)
                return new FilterNode(Id, refNode, AttributeName, ComType, Constant);
            throw new InvalidOperationException();
        }
    }
}
