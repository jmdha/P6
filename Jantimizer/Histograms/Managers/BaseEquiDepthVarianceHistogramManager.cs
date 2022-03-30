﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models;

namespace Histograms.Managers
{
    public abstract class BaseEquiDepthVarianceHistogramManager : BaseEquiDepthHistogramManager
    {
        protected BaseEquiDepthVarianceHistogramManager(ConnectionProperties connectionProperties, int depth) : base(connectionProperties, depth)
        {
            
        }
    }
}