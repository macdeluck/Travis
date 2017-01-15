using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travis.Logic.Model;

namespace Travis.Logic.Utils
{
    /// <summary>
    /// Simple implementation of <see cref="IActionSet"/>.
    /// </summary>
    public class BasicActionSet : IActionSet
    {
        public IDictionary<int, IAction> Actions { get; set; }

        public int ActionSetId { get; set; }
    }
}
