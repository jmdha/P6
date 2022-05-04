using QueryEstimator.SegmentHandler;
using Milestoner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimatorTests.Stubs
{
    internal class TestSegmentBoundsHandler : BaseSegmentBoundsHandler
    {
        public TestSegmentBoundsHandler(Dictionary<TableAttribute, int> upperBounds, Dictionary<TableAttribute, int> lowerBounds, IMilestoner milestoner) : base(upperBounds, lowerBounds, milestoner)
        {
        }
    }
}
