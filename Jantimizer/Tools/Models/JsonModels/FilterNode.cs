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
        public ComparisonType.Type ComType { get; set; }
        public IComparable Constant { get; set; }

        public FilterNode(TableAttribute filterAttribute, ComparisonType.Type comType, IComparable constant)
        {
            FilterAttribute = filterAttribute;
            ComType = comType;
            Constant = constant;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
