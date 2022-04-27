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
        public ComparisonType.Type ComType { get; internal set; }

        public JoinPredicate(TableAttribute leftAttribute, TableAttribute rightAttribute, string comType)
        {
            LeftAttribute = leftAttribute;
            RightAttribute = rightAttribute;
            ComType = ComparisonType.GetOperatorType(comType);
        }
    }
}
