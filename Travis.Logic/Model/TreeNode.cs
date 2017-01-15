﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Logic.Model
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
        /// Children nodes keyed with <see cref="IActionSet.ActionSetId"/>.
        /// </summary>
        public Dictionary<int, TreeNode> Children { get; set; } = new Dictionary<int, TreeNode>();

        /// <summary>
        /// Adds node to tree.
        /// </summary>
        /// <param name="actionSetId">Key for children node took from <see cref="IActionSet.ActionSetId"/></param>
        public TreeNode AddNode(int actionSetId)
        {
            var newNode = new TreeNode();
            Children.Add(actionSetId, newNode);
            return newNode;
        }
    }
}
