using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace Histograms.Models
{
    internal class AttributeData
    {
        public TableAttribute Attribute { get; }
        public List<ValueCount> ValueCounts { get; }
        public TypeCode TypeCode { get; }
        public AttributeData(TableAttribute attribute, List<ValueCount> valueCounts, TypeCode typeCode)
        {
            Attribute = attribute;
            ValueCounts = valueCounts;
            TypeCode = typeCode;
        }

    }
}
