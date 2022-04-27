using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class JoinPredicate
    {
        public TableAttribute LeftAttribute { get; set; }
        public TableAttribute RightAttribute { get; set; }
        public string ComType { get; set; }

        public JoinPredicate(TableAttribute leftAttribute, TableAttribute rightAttribute, string comType)
        {
            LeftAttribute = leftAttribute;
            RightAttribute = rightAttribute;
            ComType = comType;
        }
        public JoinPredicate()
        {
            LeftAttribute = new TableAttribute();
            RightAttribute = new TableAttribute();
            ComType = "";
        }
        public ComparisonType.Type GetComType()
        {
            return ComparisonType.GetOperatorType(ComType);
        }
    }
}
