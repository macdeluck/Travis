using System;
using Travis.Common.Extensions;
using Travis.Learning.Model;

namespace Travis.Learning.TreePolicies
{
    public class UCT : ITreePolicy
    {
        public UCT()
        {
            Coefficient = 1;
        }

        public double Coefficient { get; set; }

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
        /// <param name="state">A state of problem.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(TreeNode node, IState state, int actorId)
        {
            var actions = state.GetActionsForActor(actorId);
            foreach (var action in actions.Values)
            {
                if (node.Quality.ActionQuality(action.ActorId, action.ActionId).NumSelected == 0)
                    return action;
            }
            return actions.Values.ArgMax(action =>
            {
                var actionInfo = node.Quality.ActionQuality(action.ActorId, action.ActionId);
                var q = CalculateUCT(Coefficient, actionInfo.Quality, actionInfo.NumSelected, node.Quality.NumVisited);
                return q;
            }).RandomElement();
        }
    }
}
