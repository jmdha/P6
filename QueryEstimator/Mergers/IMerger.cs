using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Models.JsonModels;

namespace QueryEstimator.Mergers
{
    public interface IMerger<TBounds, TResult, TInput>
    {
        public TBounds UpperBounds { get; }
        public TBounds LowerBounds { get; }
        public TResult Stitch(TInput dict);
    }
}
