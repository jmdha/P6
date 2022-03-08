using QueryEditorTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryEditorTool.QueryModifiers
{
    public class QueryModifier : IQueryModifier
    {
        public INode ReorderMovableNodes(INode baseTree, int indexA, int indexB)
        {
            INode node1 = FindMovableNode(baseTree, indexA);
            INode node2 = FindMovableNode(baseTree, indexB);


            return baseTree;
        }

        private INode FindMovableNode(INode parent, int index)
        {
            if (parent is IMovable movable)
            {
                if (movable.ItemIndex == index)
                    return parent;
            }
            foreach (PropertyInfo propertyInfo in parent.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.IsInterface)
                {
                    INode? isNode = propertyInfo.GetValue(parent, null) as INode;
                    if (isNode != null)
                    {
                        INode returnVal = FindMovableNode(isNode, index);
                        if (returnVal != null)
                            return returnVal;
                    }
                }
            }
            return null;
        }
    }
}
