using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace Histograms.Managers
{
    public abstract class BaseEquiDepthHistogramManager : BaseHistogramManager
    {
        public int Depth { get; }

        protected BaseEquiDepthHistogramManager(ConnectionProperties connectionProperties, int depth) : base(connectionProperties)
        {
            Depth = depth;
        }
    }
}
