using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Learning.Model
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
