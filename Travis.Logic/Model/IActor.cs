using System.Collections.Generic;
using Travis.Logic.Model;

namespace Travis.Logic.Model
{
    /// <summary>
    /// Interface representing actor which takes part in contests.
    /// </summary>
    public interface IActor
    {
        /// <summary>
        /// Method called on actor when match begins.
        /// </summary>
        /// <param name="actorId">Identifier assigned to player.</param>
        /// <param name="game">Game which is executed.</param>
        void OnMatchBegin(int actorId, IGame game);

        /// <summary>
        /// Returns identifier assigned on match begin.
        /// </summary>
        int ActorId { get; }

        /// <summary>
        /// Method called when actor is asked to choose his action in given state.
        /// </summary>
        /// <param name="state">State in which actor should choose his action.</param>
        IAction SelectAction(IState state);

        /// <summary>
        /// Method called when all actors already choosed their actions.
        /// </summary>
        /// <param name="state">State which will be <paramref name="actionSet"/> applied to.</param>
        /// <param name="actionSet">Actions selected by actors which will be applied to <paramref name="state"/>.</param>
        void OnStateTransition(IState state, ActionSet actionSet);

        /// <summary>
        /// Method called when game has been finished.
        /// </summary>
        /// <param name="payoffs">Payoffs received from game.</param>
        void OnMatchFinished(IDictionary<int, double> payoffs);
    }
}
