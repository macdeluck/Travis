using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Logic.Model
{
    /// <summary>
    /// Set of actions.
    /// </summary>
    public interface IActionSet
    {
        /// <summary>
        /// Index of action set used as key in <see cref="TreeNode"/>.
        /// </summary>
        int ActionSetId { get; }

        /// <summary>
        /// Actions keyed with actor index.
        /// </summary>
        IDictionary<int, IAction> Actions { get; }
    }
}
