using QueryEditorTool.Models;
using QueryEditorTool.Models.Constants;
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

            if (node1 is JOINStmt join1)
                if (node2 is JOINStmt join2)
                    swapJoin(join1, join2);

            var tableList = new List<string>();
            StitchAllJoins(baseTree, tableList);

            return baseTree;
        }

        private void swapJoin(JOINStmt n1, JOINStmt n2)
        {
            INode value1 = n1.Value;
            INode value2 = n2.Value;
            INode left1 = n1.Left;
            INode left2 = n2.Left;
            INode right1 = n1.Right;
            INode right2 = n2.Right;
            INode parent1 = n1.Parent;
            INode parent2 = n2.Parent;

            if (parent1 == n2)
                parent1 = n1;
            if (parent2 == n1)
                parent2 = n2;

            if (left1 == n2)
            {
                left1 = null;
                left2 = n2;
            }
            if (left2 == n1)
            {
                left2 = null;
                left1 = n1;
            }

            if (right1 == n2)
            {
                right1 = null;
                right2 = n2;
            }
            if (right2 == n1)
            {
                right2 = null;
                right1 = n1;
            }

            n1.Left = left2;
            n2.Left = left1;

            n1.Right = right2;
            n2.Right = right1;

            n1.Parent = parent2;
            n2.Parent = parent1;

            n1.Value = value2;
            n2.Value = value1;

            if (n1.Left == null || n1.Right == null)
                StitchInnerJoin(n1);

            if (n2.Left == null || n2.Right == null)
                StitchInnerJoin(n2);
        }

        private void StitchInnerJoin(JOINStmt A)
        {
            if (A.Right == null)
            {
                if (A.Left is ConstVal val)
                {
                    foreach (var usedVals in A.UsedTables)
                    {
                        if (usedVals.Value != val.Value)
                        {
                            A.Right = new ConstVal(A, usedVals.Value);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (A.Left == null)
                {
                    if (A.Right is ConstVal val)
                    {
                        foreach (var usedVals in A.UsedTables)
                        {
                            if (usedVals.Value != val.Value)
                            {
                                A.Left = new ConstVal(A, usedVals.Value);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void StitchAllJoins(INode node, List<string> currentTables)
        {
            if (node is JOINStmt join)
            {
                if (join.Left is ConstVal Lval)
                {
                    currentTables.Add(Lval.Value);
                }
                else
                    StitchAllJoins(join.Left, currentTables);
                if (join.Right is ConstVal Rval)
                {
                    currentTables.Add(Rval.Value);
                }
                else
                    StitchAllJoins(join.Right, currentTables);
                string? missingTable = GetMissingTable(join.Value, currentTables);
                if (missingTable != null)
                {
                    if (join.Left is ConstVal Lval2)
                    {
                        Lval2.Value = missingTable;
                        return;
                    }
                    if (join.Right is ConstVal Rval2)
                    {
                        Rval2.Value = missingTable;
                        return;
                    }
                }
            }
            else 
            {
                if (node is IExp exp)
                {
                    StitchAllJoins(exp.Left, currentTables);
                    StitchAllJoins(exp.Right, currentTables);
                }
                if (node is IValue val)
                {
                    StitchAllJoins(val.Value, currentTables);
                }
            }
        }

        private string? GetMissingTable(INode onCondition, List<string> currentTables)
        {
            if (onCondition is ConstVal val)
            {
                string tabName = val.Value.Split('.')[0];
                if (!currentTables.Contains(tabName))
                    return tabName;
            }
            if (onCondition is IExp exp)
            {
                var node = GetMissingTable(exp.Left, currentTables);
                if (node != null)
                    return node;
                node = GetMissingTable(exp.Right, currentTables);
                if (node != null)
                    return node;
            }
            return null;
        }

        private INode FindMovableNode(INode parent, int index)
        {
            if (parent is IMovable movable)
            {
                if (movable.ItemIndex == index)
                    return parent;
            }
            if (parent is IExp exp)
            {
                var node = FindMovableNode(exp.Right, index);
                if (node != null)
                    return node;
                node = FindMovableNode(exp.Left, index);
                if (node != null)
                    return node;
            }
            if (parent is IValue val)
            {
                var node = FindMovableNode(val.Value, index);
                if (node != null)
                    return node;
            }

            return null;
        }
    }
}
