using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.Models
{
    public interface IStmt : INode
    {
        public INode Child { get; set; }
    }
}
