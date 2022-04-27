using QueryOptimiser.Cost.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiser.Cost.EstimateCalculators.MatchFinders
{
    public interface IMatchFinder<EstimatorType>
        where EstimatorType : INodeEstimate
    {
    }
}
