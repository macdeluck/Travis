using Travis.Common.Model;

namespace Travis.Learning.Model
{
    /// <summary>
    /// Represents default policy.
    /// </summary>
    public interface IDefaultPolicy
    {
        /// <summary>
        /// Selects action for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        IAction Invoke(IState state, int actorId);
    }
}
