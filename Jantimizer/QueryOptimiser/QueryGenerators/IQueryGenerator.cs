using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryParser.Models;

namespace QueryOptimiser.QueryGenerators
{
    public interface IQueryGenerator
    {
        public string GenerateQuery(List<INode> nodes);
        public string GenerateQuery(List<Tuple<INode, int>> nodes);
    }
}
