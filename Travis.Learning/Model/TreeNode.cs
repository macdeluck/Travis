using System.Collections.Generic;

namespace Travis.Learning.Model
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
        /// Adds node to tree.
        /// </summary>
        /// <param name="actionSetId">Key for children node took from <see cref="ActionSet.ActionSetId"/></param>
        public TreeNode AddNode(int actionSetId)
        {
            var newNode = new TreeNode();
            Children.Add(actionSetId, newNode);
            return newNode;
        }
    }
}
