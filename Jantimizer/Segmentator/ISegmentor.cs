using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Segmentator
{
    public interface ISegmentor
    {
        public Task<List<Task>> AddSegmentsFromDB();
        public void ClearSegments();
    }
}
