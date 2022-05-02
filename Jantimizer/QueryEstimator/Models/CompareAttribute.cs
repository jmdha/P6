using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Models
{
    internal class CompareAttribute
    {
        public TableAttribute Attribute { get; set; }
        public ComparisonType.Type Type { get; set; }

        public CompareAttribute(TableAttribute attribute, ComparisonType.Type type)
        {
            Attribute = attribute;
            Type = type;
        }
    }
}
