using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe.Heuristics
{
    /// <summary>
    /// Policy to select winning board only actions.
    /// </summary>
    public class WinningBoardOnly : IDefaultPolicy
    {
        /// <summary>
        /// Selects action for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(IState state, int actorId)
        {
            var mtttState = state as MultipleTicTacToeState;
            if (mtttState.ControlPlayer != actorId)
                return state.GetActionsForActor(actorId).Values.First();
            return state.GetActionsForActor(actorId).Values.Where(a => (a as MultipleTicTacToeAction).BoardNum == MultipleTicTacToeState.WinningBoard).RandomElement();
        }
    }
}
