using System;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Logic.MCTS
{
    /// <summary>
    /// Default policy which mixes two stand-alone default policies.
    /// </summary>
    public class ProbabilityMixedDefaultPolicy : IDefaultPolicy
    {
        /// <summary>
        /// The probability threshold.
        /// </summary>
        public double ProbabilityThreshold { get; set; } = 0;

        /// <summary>
        /// The original policy.
        /// </summary>
        public IDefaultPolicy OriginalPolicy { get; set; } = new RandomDefaultPolicy();

        /// <summary>
        /// The additional policy.
        /// </summary>
        public IDefaultPolicy AdditionalPolicy { get; set; } = new RandomDefaultPolicy();

        /// <summary>
        /// Random values selector.
        /// </summary>
        public Random Random { get; set; } = new Random();

        /// <summary>
        /// Selects action for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(IState state, int actorId)
        {
            return Random.NextDouble() > ProbabilityThreshold
                ? AdditionalPolicy.Invoke(state, actorId)
                : OriginalPolicy.Invoke(state, actorId);
        }
    }
}
