using Travis.Common.Extensions;
using Travis.Common.Model;

namespace Travis.Learning.Model
{
    /// <summary>
    /// Default policy with selects moves randomly.
    /// </summary>
    public class RandomDefaultPolicy : IDefaultPolicy
    {
        /// <summary>
        /// Selects action randomly for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of problem.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(IState state, int actorId)
        {
            return state.GetActionsForActor(actorId).Values.RandomElement();
        }
    }
}
