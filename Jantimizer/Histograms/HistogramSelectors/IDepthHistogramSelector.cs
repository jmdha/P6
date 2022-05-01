using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.HistogramSelectors
{
    public interface IDepthHistogramSelector<T> : IHistogramSelector<T> where T : IDepthHistogram
    {
        public T GetHistogramDepthOfTypeOrAlt(T targetType, Type type, string tableName, string attributeName, DepthCalculator depth);
    }
}
