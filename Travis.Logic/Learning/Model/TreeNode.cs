using System;
using System.Collections.Generic;
using Travis.Logic.Model;

namespace Travis.Logic.Learning.Model
{
    /// <summary>
    /// Represents node of tree.
    /// </summary>
    public class TreeNode
    {
        /// <summary>
        /// Stores node quality info.
        /// </summary>
        public NodeQualityInfo Quality { get; set; } = new NodeQualityInfo();

        /// <summary>
        /// Children nodes keyed with <see cref="ActionSet.ActionSetId"/>.
        /// </summary>
        public Dictionary<int, TreeNode> Children { get; set; } = new Dictionary<int, TreeNode>();

        /// <summary>
        /// Returns true if node is terminal. Terminal nodes cannot be expanded.
        /// </summary>
        public bool IsTerminal { get; private set; }

        /// <summary>
        /// Adds node to tree.
        /// </summary>
        /// <param name="actionSetId">Key for children node took from <see cref="ActionSet.ActionSetId"/></param>
        /// <param name="isTerminal">If true, node cannot be expanded.</param>
        public TreeNode AddNode(int actionSetId, bool isTerminal = false)
        {
            if (IsTerminal)
                throw new InvalidOperationException(Messages.TryingToExpandTerminalNode);
            var newNode = new TreeNode() { IsTerminal = isTerminal };
            Children.Add(actionSetId, newNode);
            return newNode;
        }
    }
}
