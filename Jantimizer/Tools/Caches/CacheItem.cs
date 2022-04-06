using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Caches
{
    public class CacheItem
    {
        public string Hash { get; set; }
        public string Content { get; set; }
        public string CacherServiceName { get; set; }

        public CacheItem(string hash, string content, string cacherServiceName)
        {
            Hash = hash;
            Content = content;
            CacherServiceName = cacherServiceName;
        }
    }
}
