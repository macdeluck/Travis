using System;
using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;
using System.Linq;

namespace Travis.Logic.MCTS
{
    /// <summary>
    /// Represents tree policy using UCT algorithm.
    /// </summary>
    public class UCT : ITreePolicy
    {
        /// <summary>
        /// Coefficient used to change balance between exploration and exploitation.
        /// </summary>
        public double Coefficient { get; set; } = 1;

        /// <summary>
        /// Calculates UCT value.
        /// </summary>
        /// <param name="c">UCT coefficient.</param>
        /// <param name="qa">Quality of considered action.</param>
        /// <param name="na">Number how many times considered action was selected.</param>
        /// <param name="n">Number how many times considered node was visited.</param>
        public static double CalculateUCT(double c, double qa, int na, int n)
        {
            if (n == 0 || na == 0)
                return qa;
            return qa + c * Math.Sqrt(Math.Log(n) / na);
        }

        /// <summary>
        /// Selects action for particular actor for given state within game tree using UCT algorithm.
        /// </summary>
        /// <param name="node">TreeNode refering to <paramref name="state"/>.</param>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(TreeNode node, IState state, int actorId)
        {
            var actions = state.GetActionsForActor(actorId);
            foreach (var action in actions.Values)
            {
                if (!node.Quality.ContainsActionQuality(action.ActorId, action.ActionId))
                    return action;
            }
            return actions.Values.ArgMax(action =>
            {
                var actionInfo = node.Quality.ActorActionsQualities[action.ActorId][action.ActionId];
                var q = CalculateUCT(Coefficient, actionInfo.Quality, actionInfo.NumSelected, node.Quality.NumVisited);
                return q;
            }).RandomElement();
        }
    }
}
