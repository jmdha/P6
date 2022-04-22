using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public class FilterNode : INode
    {
        public string Alias { get; set; }
        public string AttributeName { get; set; }
        public ComparisonType.Type ComType { get; set; }
        public IComparable Constant { get; set; }
        public int Id { get; private set; }

        public FilterNode(int id, string alias, string attributeName, ComparisonType.Type comType, IComparable constant)
        {
            Id = id;
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
            return HashCode.Combine(Id, Alias, AttributeName, ComparisonType.GetOperatorString(ComType), Constant);
        }

        public object Clone()
        {
            return new FilterNode(Id, Alias, AttributeName, ComType, Constant);
        }
    }
}
