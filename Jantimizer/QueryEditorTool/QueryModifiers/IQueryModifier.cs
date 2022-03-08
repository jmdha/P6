using QueryEditorTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.QueryModifiers
{
    public interface IQueryModifier
    {
        public INode ReorderMovableNodes(INode baseTree, int indexA, int indexB);
    }
}
