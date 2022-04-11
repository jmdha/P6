using Histograms.Exceptions;
using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.HistogramSelectors
{
    public class HistogramSelector : IHistogramSelector<IHistogram>
    {
        public HistogramSelector()
        {
        }

        public IHistogram GetHistogramDepthOfTypeOrAlt(IHistogram targetType, Type type, string tableName, string attributeName)
        {
            throw new NotImplementedException();
        }
    }
}
