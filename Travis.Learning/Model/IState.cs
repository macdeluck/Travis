using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Learning.Model
{
    /// <summary>
    /// Represents state of problem.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Returns available actions to take by actor.
        /// </summary>
        IDictionary<int, IAction> GetActionsForActor(int actorId);

        /// <summary>
        /// Clones itself.
        /// </summary>
        IState Clone();

        /// <summary>
        /// Applies action set to state and switches to next state.
        /// </summary>
        /// <param name="actionSet">A set of actions taken by actors.</param>
        void Apply(ActionSet actionSet);

        /// <summary>
        /// Creates action set from chosen actions.
        /// </summary>
        /// <param name="actions">Actions chosen by actors keyed with their ids.</param>
        ActionSet CreateActionSet(IDictionary<int, IAction> actions);

        /// <summary>
        /// Returns true if state is terminal.
        /// </summary>
        bool IsTerminal { get; }

        /// <summary>
        /// Gets payoffs for terminal state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Should be thrown when method called on non terminal state.</exception>
        IDictionary<int, double> GetPayoffs();
    }
}
