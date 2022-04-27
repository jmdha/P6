using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class FilterNode : INode
    {
        public TableAttribute FilterAttribute { get; set; }
        public string ComType { get; set; }
        public IComparable Constant { get; set; }

        public FilterNode(TableAttribute filterAttribute, string comType, IComparable constant)
        {
            FilterAttribute = filterAttribute;
            ComType = comType;
            Constant = constant;
        }

        public FilterNode()
        {
            FilterAttribute = new TableAttribute();
            ComType = "";
            Constant = "";
        }

        public ComparisonType.Type GetComType()
        {
            return ComparisonType.GetOperatorType(ComType);
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object? obj)
        {
            return obj is FilterNode node &&
                   EqualityComparer<TableAttribute>.Default.Equals(FilterAttribute, node.FilterAttribute) &&
                   ComType == node.ComType &&
                   EqualityComparer<IComparable>.Default.Equals(Constant, node.Constant);
        }

        public override int GetHashCode()
        {
            return FilterAttribute.GetHashCode() + HashCode.Combine(ComType, Constant);
        }
    }
}
