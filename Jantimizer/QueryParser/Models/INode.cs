using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryParser.Models
{
    public interface INode
    {
        public int Id { get; }
        public string GetSuffixString();
    }
}
