using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    public interface IExp : INode
    {
        public INode? Left { get; set; }
        public INode? Right { get; set; }
    }
}
