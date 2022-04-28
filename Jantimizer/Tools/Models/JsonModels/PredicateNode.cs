using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class PredicateNode : ICloneable
    {
        public TableAttribute? Attribute { get; set; }
        public string? ConstantValue { get; set; }

        public PredicateNode(TableAttribute attribute)
        {
            Attribute = attribute;
            ConstantValue = null;
        }

        public PredicateNode(string constantValue)
        {
            Attribute = null;
            ConstantValue = constantValue;
        }

        public PredicateNode()
        {
            Attribute = null;
            ConstantValue = null;
        }

        public object Clone()
        {
            if (Attribute != null)
                if (Attribute.Clone() is TableAttribute att)
                    return new PredicateNode(att);
            if (ConstantValue != null)
                return new PredicateNode(ConstantValue);
            throw new ArgumentException("Could not clone");
        }

        public override string? ToString()
        {
            if (Attribute != null)
                return $"Attribute: {Attribute}";
            if (ConstantValue != null)
                return $"Constant: {ConstantValue}";
            return "";
        }
    }
}
