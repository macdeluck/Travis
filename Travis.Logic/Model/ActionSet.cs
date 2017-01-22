using System.Collections.Generic;

namespace Travis.Logic.Model
{
    /// <summary>
    /// Set of actions.
    /// </summary>
    public class ActionSet
    {
        /// <summary>
        /// Index of action set used as key in <see cref="TreeNode"/>.
        /// </summary>
        public int ActionSetId { get; set; }

        /// <summary>
        /// Actions keyed with actor index.
        /// </summary>
        public IDictionary<int, IAction> Actions { get; set; }
    }
}
