using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Logic.MCTS
{
    /// <summary>
    /// Default policy with selects moves randomly.
    /// </summary>
    public class RandomDefaultPolicy : IDefaultPolicy
    {
        /// <summary>
        /// Selects action randomly for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(IState state, int actorId)
        {
            return state.GetActionsForActor(actorId).Values.RandomElement();
        }
    }
}
