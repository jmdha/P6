using Histograms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Histograms.HistogramSelectors
{
    public interface IHistogramSelector<T> where T : IHistogram
    {
        public T GetHistogramOfTypeOrAlt(T targetType, Type type, string tableName, string attributeName);
    }
}
