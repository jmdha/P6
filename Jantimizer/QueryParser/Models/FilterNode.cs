using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class FilterNode : INode
    {
        public int Id { get; set; }
        public TableReferenceNode TableReference { get; set; }
        public string AttributeName { get; set; }
        public ComparisonType.Type ComType { get; set; }
        public IComparable Constant { get; set; }

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
    }
}
