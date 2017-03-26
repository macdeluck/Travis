using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Games.FarmingQuandaries.Heuristics
{
    /// <summary>
    /// Heuristic which chooses action which may increase points.
    /// </summary>
    public class FarmingQuandariesPoints : IDefaultPolicy
    {
        /// <summary>
        /// Selects action for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(IState state, int actorId)
        {
            var fqstate = state as FarmingQuandariesState;
            var actions = fqstate.GetActionsForActor(actorId);
            if (fqstate.SeasonIndex == FarmingQuandariesSeason.Fall && actorId == fqstate.ControlPlayer)
            {
                int[] rowsCounts = new int[fqstate.Board.GetLength(0)];
                int[] colsCounts = new int[fqstate.Board.GetLength(1)];
                for (int x = 0; x < fqstate.Board.GetLength(0); x++)
                {
                    for (int y = 0; y < fqstate.Board.GetLength(1); y++)
                    {
                        var add = fqstate.Board[x, y] == 3 ? 1 : 0;
                        rowsCounts[y] += add;
                        colsCounts[x] += add;
                        if (rowsCounts[y] >= 3)
                            return actions.Values.OfType<FarmingQuandariesAction>().First(fa => fa.IsRowAction && fa.Index == y);
                        if (colsCounts[x] >= 3)
                            return actions.Values.OfType<FarmingQuandariesAction>().First(fa => !fa.IsRowAction && fa.Index == x);
                    }
                }
            }
            return GetRandomAction(actions.Values);
        }

        /// <summary>
        /// Indicates that random action should be chosen.
        /// </summary>
        /// <param name="actions">Collection of available actions.</param>
        protected virtual IAction GetRandomAction(ICollection<IAction> actions)
        {
            return actions.RandomElement();
        }
    }
}
