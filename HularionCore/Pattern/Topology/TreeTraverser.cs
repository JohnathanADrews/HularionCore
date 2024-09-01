#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace HularionCore.Pattern.Topology
{
    /// <summary>
    /// Traverses a tree to provide information or execute actions against node.s
    /// </summary>
    /// <typeparam name="NodeType">The type of node in the tree.</typeparam>
    public class TreeTraverser<NodeType>
        where NodeType : class
    {

        private static Action<NodeState> defaultAction = new Action<NodeState>(node => { });

        /// <summary>
        /// A generic tree processor that can be used to process trees in any parent-edge order.
        /// </summary>
        /// <param name="root">The root node.</param>
        /// <param name="childSelector">Returns the child nodes of a node or an empty array if there are none.</param>
        /// <param name="processor">Processes the nodes in order.</param>
        /// <param name="parentFirst">True iff the parent should be selected before the child nodes.</param>
        private void TraverseTreePrivate(NodeType root, Func<NodeType, NodeType[]> childSelector, Func<NodeType, bool> processor, bool parentFirst, Action<NodeType> popProcessor = null)
        {
            var nodeStack = new Stack<NodeType>();
            var indices = new Stack<int>();
            var childNodes = new Dictionary<NodeType, NodeType[]>();
            var crossHash = new HashSet<NodeType>();
            NodeType[] children;

            nodeStack.Push(root);
            indices.Push(0);

            while (true)
            {
                var node = nodeStack.Peek();
                var index = indices.Peek();

                if (parentFirst && index == 0 && !processor(node))
                {
                    return;
                }

                if (childNodes.ContainsKey(node))
                {
                    children = childNodes[node];
                }
                else
                {
                    children = childSelector(node);
                    childNodes[node] = children;
                }

                if (index >= children.Length)
                {
                    if (!parentFirst && !processor(node))
                    {
                        return;
                    }
                    if (node == root)
                    {
                        break;
                    }
                    if (popProcessor != null) { popProcessor(node); }
                    nodeStack.Pop();
                    indices.Pop();
                    var next = indices.Pop() + 1;
                    indices.Push(next);
                    continue;
                }
                if (crossHash.Contains(children[index]))
                {
                    throw new ArgumentException("The provided nodes contain a cross-reference.");
                }
                crossHash.Add(children[index]);
                nodeStack.Push(children[index]);
                indices.Push(0);
            }
        }

        /// <summary>
        /// Traverses the tree with the provided root node and processes nodes in the specified order.
        /// </summary>
        /// <param name="order">The order in which to process nodes in the tree.</param>
        /// <param name="root">The root node of the tree.</param>
        /// <param name="childSelector">Provides the child nodes of a given node.  The "left" node must be at the zero index.</param>
        /// <param name="processor">Processes each node in order until there are no remaining nodes or the processor returns false.</param>
        public void TraverseTree(TreeTraversalOrder order, NodeType root, Func<NodeType, NodeType[]> childSelector, Func<NodeType, bool> processor, Action<NodeType> popProcessor = null)
        {
            //The reverse selector orders the nodes for right to left traversal.
            Func<NodeType, NodeType[]> reverseSelector;
            switch (order)
            {
                case TreeTraversalOrder.LeftRightParent:
                    TraverseTreePrivate(root, childSelector, processor, false, popProcessor: popProcessor);
                    break;
                case TreeTraversalOrder.RightLeftParent:
                    reverseSelector = new Func<NodeType, NodeType[]>(node => childSelector(node).Reverse().ToArray());
                    TraverseTreePrivate(root, reverseSelector, processor, false, popProcessor: popProcessor);
                    break;
                case TreeTraversalOrder.ParentLeftRight:
                    TraverseTreePrivate(root, childSelector, processor, true, popProcessor: popProcessor);
                    break;
                case TreeTraversalOrder.ParentRightLeft:
                    reverseSelector = new Func<NodeType, NodeType[]>(node => childSelector(node).Reverse().ToArray());
                    TraverseTreePrivate(root, reverseSelector, processor, true, popProcessor: popProcessor);
                    break;
            }
        }

        /// <summary>
        /// Creates an array of NodeType that can be used to evaluate the tree as spefified by the order.       
        /// </summary>
        /// <param name="order">The order in which nodes will appear in the evaluation plan.</param>
        /// <param name="root">The root node of the tree.</param>
        /// <param name="childSelector">Provides the child nodes of a given node.  The "left" node must be at the zero index.</param>
        /// <param name="includeLeaves">Iff tru, the leaf nodes are included in the evaluation plan.</param>
        /// <returns>An array of NodeType representing a sequence for evaluating the nodes.</returns>
        public NodeType[] CreateEvaluationPlan(TreeTraversalOrder order, NodeType root, Func<NodeType, NodeType[]> childSelector, bool includeLeaves)
        {
            var result = new List<NodeType>();
            var childNodes = new Dictionary<NodeType, NodeType[]>();
            var cacheSelector = new Func<NodeType, NodeType[]>(node =>
            {
                NodeType[] children;
                if (childNodes.ContainsKey(node))
                {
                    children = childNodes[node];
                }
                else
                {
                    children = childSelector(node);
                    childNodes[node] = children;
                }
                return children;
            });

            var addProcessor = new Func<NodeType, bool>(node =>
            {
                if (includeLeaves || cacheSelector(node).Length > 0)
                {
                    result.Add(node);
                }
                return true;
            });

            TraverseTree(order, root, cacheSelector, addProcessor);
            return result.ToArray();
        }

        /// <summary>
        /// Creates an array of NodeType that can be used to evaluate the tree as spefified by the order.       
        /// </summary>
        /// <param name="order">The order in which nodes will appear in the evaluation plan.</param>
        /// <param name="roots">The root nodes of the tree.</param>
        /// <param name="childSelector">Provides the child nodes of a given node.  The "left" node must be at the zero index.</param>
        /// <param name="includeLeaves">Iff tru, the leaf nodes are included in the evaluation plan.</param>
        /// <returns>An array of NodeType representing a sequence for evaluating the nodes.</returns>
        public NodeType[] CreateEvaluationPlan(TreeTraversalOrder order, NodeType[] roots, Func<NodeType, NodeType[]> childSelector, bool includeLeaves)
        {
            var result = new List<NodeType>();
            for (var i = 0; i < roots.Length; i++)
            {
                var root = roots[i];
                result.AddRange(CreateEvaluationPlan(order, root, childSelector, includeLeaves));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Creates an evaluation plan that ignores duplicate nodes, handling just the first encounter.
        /// </summary>
        /// <param name="order">The order in which nodes will appear in the evaluation plan.</param>
        /// <param name="root">The root node of the tree.</param>
        /// <param name="childSelector">Provides the child nodes of a given node.  The "left" node must be at the zero index.</param>
        /// <param name="includeLeaves">Iff true, the leaf nodes are included in the evaluation plan.</param>
        /// <returns>An array of NodeType representing a sequence for evaluating the nodes.</returns>
        public NodeType[] CreateUniqueNodeEvaluationPlan(TreeTraversalOrder order, NodeType root, Func<NodeType, NodeType[]> childSelector, bool includeLeaves)
        {
            var unique = new HashSet<NodeType>();
            unique.Add(root);
            return CreateEvaluationPlan(order, root, node =>
            {
                var next = childSelector(node);
                var result = new List<NodeType>();
                foreach(var item in next)
                {
                    if (!unique.Contains(item)) { result.Add(item); }
                    unique.Add(item);
                }
                return result.ToArray();
            }, includeLeaves);
        }

        /// <summary>
        /// Creates an evaluation plan that ignores duplicate nodes, handling just the first encounter.
        /// </summary>
        /// <param name="order">The order in which nodes will appear in the evaluation plan.</param>
        /// <param name="roots">The root nodes of the tree.</param>
        /// <param name="childSelector">Provides the child nodes of a given node.  The "left" node must be at the zero index.</param>
        /// <param name="includeLeaves">Iff true, the leaf nodes are included in the evaluation plan.</param>
        /// <returns>An array of NodeType representing a sequence for evaluating the nodes.</returns>
        public NodeType[] CreateUniqueNodeEvaluationPlan(TreeTraversalOrder order, NodeType[] roots, Func<NodeType, NodeType[]> childSelector, bool includeLeaves)
        {
            var result = new List<NodeType>();
            var unique = new HashSet<NodeType>();
            for (var i = 0; i < roots.Length; i++)
            {
                var root = roots[i];
                unique.Add(root);
                var plan = CreateEvaluationPlan(order, root, node =>
                 {
                     var next = childSelector(node);
                     var nodes = new List<NodeType>();
                     foreach (var item in next)
                     {
                         if (!unique.Contains(item)) { nodes.Add(item); }
                         unique.Add(item);
                     }
                     return nodes.ToArray();
                 }, includeLeaves);
                result.AddRange(plan);
            }
            return result.ToArray();
        }


        /// <summary>
        /// Weaves through the tree, executing one of the actions as the reader transitions from one node to the next.
        /// On leaving a node, exitAction(node) is called first.  Then lastAction(node) if the node is the last of it's siblings.  Finally, upAction(node.Parent) is called if node is not root.
        /// </summary>
        /// <param name="order">Starts the weave from left or right.</param>
        /// <param name="root">The root node of the tree.</param>
        /// <param name="childSelector">Selects the children of a node.</param>
        /// <param name="entryAction">Executes when the reader first enters a node.</param>
        /// <param name="exitAction">Executes when the reader leaves a node for good, moving to its sibling or parent.</param>
        /// <param name="lastAction">Executes when the reader leaves the last node in an array of nodes.</param>
        /// <param name="upAction">Executes when the reader transitions from a node to its parent.</param>
        public void WeaveExecute(TreeWeaveOrder order, NodeType root, Func<NodeType, NodeType[]> childSelector,
            Action<NodeState> entryAction = null, 
            Action<NodeState> exitAction = null,
            Action<NodeState> lastAction = null,
            Action<NodeState> upAction = null)
        {

            if (entryAction == null) { entryAction = defaultAction; }
            if (exitAction == null) { exitAction = defaultAction; }
            if (lastAction == null) { lastAction = defaultAction; }
            if (upAction == null) { upAction = defaultAction; }

            //Setup the correct traversal order based on weave order.
            var traverseOrder = TreeTraversalOrder.ParentLeftRight;
            if (order == TreeWeaveOrder.FromRight)
            {
                traverseOrder = TreeTraversalOrder.ParentRightLeft;
            }

            var childrenMap = new Dictionary<NodeType, NodeType[]>();
            var stateMap = new Dictionary<NodeType, NodeState>();

            //Initialize root.
            var rootChildren = childSelector(root);
            childrenMap.Add(root, rootChildren);
            stateMap.Add(root, new NodeState()
            {
                Subject = root,
                Parent = null,
                IsRoot = true,
                IsLeaf = rootChildren.Length == 0 ? true : false,
                IsRowFirst = true,
                IsRowLast = true,
                RowIndex = 0,
                RowLength = 1,
                Depth = 0
            });

            TraverseTree(traverseOrder, root,
                node =>
                {
                    //Here we need to setup all the grandchildren state information because IsLeaf needs to be set prior to the traverser going into the node.
                    var children = childrenMap[node];
                    var parentState = stateMap[node];
                    NodeType[] grandChildren;
                    int index = 0;
                    var childStates = new List<NodeState>();
                    foreach (var child in children)
                    {
                        grandChildren = childSelector(child);
                        childrenMap.Add(child, grandChildren);
                        var childState = new NodeState()
                        {
                            Subject = child,
                            Parent = node,
                            IsRoot = false,
                            IsLeaf = grandChildren.Length == 0 ? true : false,
                            IsRowFirst = index == 0,
                            IsRowLast = index == children.Length - 1,
                            RowIndex = index,
                            RowLength = children.Length,
                            Depth = parentState.Depth + 1,
                            ParentState = parentState
                        };
                        childStates.Add(childState);
                        stateMap.Add(child, childState);
                        index++;
                    }
                    parentState.ChildrenStates = childStates.ToArray();
                    return children;
                },
                new Func<NodeType, bool>(node=>
                {
                    entryAction(stateMap[node]);
                    return true;
                }),
                popProcessor: new Action<NodeType>(node=>
                {
                    var state = stateMap[node];
                    exitAction(state);
                    if (childrenMap[state.Parent].Last() == node)
                    {
                        lastAction(state);
                    }
                    if (!state.IsRoot)
                    {
                        upAction(stateMap[state.Parent]);
                    }
                }));
            exitAction(stateMap[root]);
            lastAction(stateMap[root]);
        }


        /// <summary>
        /// The state of a node to handle.
        /// </summary>
        public class NodeState
        {
            /// <summary>
            /// The node currently under examination.
            /// </summary>
            public NodeType Subject { get; set; }
            /// <summary>
            /// The node that provided the row this node belongs to.
            /// </summary>
            public NodeType Parent { get; set; }
            /// <summary>
            /// The NodeState of the parent.
            /// </summary>
            public NodeState ParentState { get; set; }
            /// <summary>
            /// The child NodeState objects.
            /// </summary>
            public NodeState[] ChildrenStates { get; set; } = new NodeState[] { };
            /// <summary>
            /// True iff this no has no parent node.
            /// </summary>
            public bool IsRoot { get; set; }
            /// <summary>
            /// True iff this nod has no child nodes.
            /// </summary>
            public bool IsLeaf { get; set; }
            /// <summary>
            /// True iff this is the first node in the row as provided by its parent.
            /// </summary>
            public bool IsRowFirst { get; set; }
            /// <summary>
            /// True iff this is the last node in the row as provided by its parent.
            /// </summary>
            public bool IsRowLast { get; set; }
            /// <summary>
            /// The zero-based index of this node relative to its row as provided by its parent.
            /// </summary>
            public int RowIndex { get; set; }
            /// <summary>
            /// The length of the row this node belongs to as provided by its parent.
            /// </summary>
            public int RowLength { get; set; }
            /// <summary>
            /// The descendant number of this node compared to root (depth 0).
            /// </summary>
            public int Depth { get; set; }
        }

    }

    /// <summary>
    /// The available traversal patterns.
    /// </summary>
    public enum TreeTraversalOrder
    {
        /// <summary>
        /// Nodes are selected in left, right, parent order,
        /// </summary>
        LeftRightParent,
        /// <summary>
        /// Nodes are selected in right, left, parent order,
        /// </summary>
        RightLeftParent,
        /// <summary>
        /// Nodes are selected in parent, left, right order,
        /// </summary>
        ParentLeftRight,
        /// <summary>
        /// Nodes are selected in parent, right, left order,
        /// </summary>
        ParentRightLeft
    }

    /// <summary>
    /// Indicates the direction to weave through the tree.
    /// </summary>
    public enum TreeWeaveOrder
    {
        /// <summary>
        /// Weave starting from the left.
        /// </summary>
        FromLeft,
        /// <summary>
        /// Weave starting from the right.
        /// </summary>
        FromRight
    }
}
