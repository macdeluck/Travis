using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Logic.Model
{
    /// <summary>
    /// Action of actor.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets index of action.
        /// </summary>
        int ActionId { get; }

        /// <summary>
        /// Gets index of actor.
        /// </summary>
        int ActorId { get; }
    }
}
