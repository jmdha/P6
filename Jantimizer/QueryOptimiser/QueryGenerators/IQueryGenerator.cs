using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueryOptimiser.Cost.Nodes;
using QueryParser.Models;

namespace QueryOptimiser.QueryGenerators
{
    public interface IQueryGenerator
    {
        /// <summary>
        /// Creates a query string from the parameter <paramref name="nodes"/> according to the specific implementation chosen
        /// </summary>
        /// <returns></returns>
        public string GenerateQuery(List<INode> nodes);

        /// <summary>
        /// Creates a query string from the parameter <paramref name="nodes"/> according to the specific implementation chosen
        /// <para>As opposed to <see cref="GenerateQuery(List{INode})"/> this also reorders it according to the cost of each join operation, lowest first</para>
        /// </summary>
        /// <returns></returns>
        public string GenerateQuery(List<ValuedNode> nodes);
    }
}
