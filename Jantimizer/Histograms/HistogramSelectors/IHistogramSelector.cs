using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.HistogramSelectors
{
    public interface IHistogramSelector
    {
        public IHistogram GetHistogramDepthOfTypeOrAlt(IHistogram targetType, Type type, string tableName, string attributeName, int depth);
    }
}
