using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentSuite.UserControls
{
    public interface ICollapsable
    {
        public double CollapsedSize { get; }
        public double ExpandedSize { get; }
        public void Toggle();
        public void Toggle(bool collapse);
    }
}
