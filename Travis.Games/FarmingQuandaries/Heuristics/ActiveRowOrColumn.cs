using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Games.FarmingQuandaries.Heuristics
{
    /// <summary>
    /// Selects always active row or column as action when control player.
    /// Arsons always when not control player.
    /// </summary>
    public class ActiveRowOrColumn : IDefaultPolicy
    {
        /// <summary>
        /// Player active rows.
        /// </summary>
        public int[] PlayerRowsCounts { get; set; }

        /// <summary>
        /// Player active cols.
        /// </summary>
        public int[] PlayerColsCounts { get; set; }

        /// <summary>
        /// Opponent active rows.
        /// </summary>
        public int[] OpponentRowsCounts { get; set; }

        /// <summary>
        /// Opponent active cols.
        /// </summary>
        public int[] OpponentColsCounts { get; set; }

        /// <summary>
        /// Selects action for particular actor for given state below game tree.
        /// </summary>
        /// <param name="state">A state of game.</param>
        /// <param name="actorId">Actor id action should be selected for.</param>
        public IAction Invoke(IState state, int actorId)
        {
            var fqstate = state as FarmingQuandariesState;
            var actions = fqstate.GetActionsForActor(actorId);
            if (fqstate.ControlPlayer == actorId)
            {
                InitActiveRowsAndCols(fqstate, actorId);
                if (fqstate.YearStartPlayer == actorId)
                {
                    var rowsAndCols = ActiveRowsAndCols(true).ToList();
                    if (rowsAndCols.Any())
                    {
                        var rc = rowsAndCols.RandomElement();
                        return actions.Values.OfType<FarmingQuandariesAction>().Where(fa => fa.IsRowAction == rc.Item1 && fa.Index == rc.Item2).First();
                    }
                }
                else
                {
                    var rowsAndCols = ActiveRowsAndCols(true).ToList();
                    var opponentRowsAndCols = ActiveRowsAndCols(false).ToList();
                    if (FarmingQuandariesSeason.Fall == fqstate.SeasonIndex && rowsAndCols.Any())
                    {
                        var rc = rowsAndCols.RandomElement();
                        return actions.Values.OfType<FarmingQuandariesAction>().Where(fa => fa.IsRowAction == rc.Item1 && fa.Index == rc.Item2).First();
                    }
                    else if (opponentRowsAndCols.Count == 0)
                    {
                        return actions.Values.OfType<FarmingQuandariesAction>().Where(fa => !rowsAndCols.Any(rc => rc.Item1 == fa.IsRowAction && rc.Item2 == fa.Index)).RandomElement();
                    }
                    else if (!fqstate.HasArson[actorId] && !fqstate.HasArson[actorId])
                    {
                        if (rowsAndCols.Any())
                        {
                            var rc = rowsAndCols.RandomElement();
                            return actions.Values.OfType<FarmingQuandariesAction>().Where(fa => fa.IsRowAction == rc.Item1 && fa.Index == rc.Item2).First();
                        }
                    }
                }
            }
            else
            {
                if (fqstate.HasArson[actorId])
                {
                    if (fqstate.YearStartPlayer != actorId)
                    {
                        InitActiveRowsAndCols(fqstate, actorId);
                        var opponentRowsAndCols = ActiveRowsAndCols(false).ToList();
                        if (opponentRowsAndCols.Count != 1)
                            return actions.Values.OfType<FarmingQuandariesAction>().First(fa => fa.IsNoop);
                        else
                        {
                            var rc = opponentRowsAndCols.RandomElement();
                            return actions.Values.OfType<FarmingQuandariesAction>().Where(fa => fa.IsRowAction == rc.Item1 && fa.Index == rc.Item2).First();
                        }
                    }
                    else return actions.Values.OfType<FarmingQuandariesAction>().First(fa => fa.IsNoop);
                }
            }
            return GetRandomAction(actions.Values);
        }

        private IEnumerable<Tuple<bool, int>> ActiveRowsAndCols(bool player)
        {
            int[] rowsCounts;
            int[] colsCounts;
            if (player)
            {
                rowsCounts = PlayerRowsCounts;
                colsCounts = PlayerColsCounts;
            }
            else
            {
                rowsCounts = OpponentRowsCounts;
                colsCounts = OpponentColsCounts;
            }
            for (int i = 0; i < 4; i++)
                if (rowsCounts[i] >= 3)
                {
                    yield return Tuple.Create(true, i);
                }
            for (int i = 0; i < 4; i++)
                if (colsCounts[i] >= 3)
                {
                    yield return Tuple.Create(false, i);
                }
        }

        /// <summary>
        /// Selects random action from given set.
        /// </summary>
        /// <param name="actions">Available actions</param>
        protected virtual IAction GetRandomAction(ICollection<IAction> actions)
        {
            return actions.RandomElement();
        }

        private void InitActiveRowsAndCols(FarmingQuandariesState fqstate, int actorId)
        {
            PlayerRowsCounts = new int[4];
            PlayerColsCounts = new int[4];
            OpponentRowsCounts = new int[4];
            OpponentColsCounts = new int[4];
            int plAdd;
            int opAdd;
            if (fqstate.YearStartPlayer == actorId)
            {
                if (fqstate.ControlPlayer == actorId)
                {
                    plAdd = 0;
                    opAdd = 0;
                }
                else
                {
                    plAdd = 1;
                    opAdd = 0;
                }
            }
            else
            {
                if (fqstate.ControlPlayer == actorId)
                {
                    plAdd = 0;
                    opAdd = 1;
                }
                else
                {
                    plAdd = 0;
                    opAdd = 0;
                }
            }
            for (int x = 0; x < fqstate.Board.GetLength(0); x++)
            {
                for (int y = 0; y < fqstate.Board.GetLength(1); y++)
                {
                    if (fqstate.Board[x, y] == fqstate.SeasonIndex + plAdd)
                    {
                        PlayerRowsCounts[y]++;
                        PlayerColsCounts[x]++;
                    }
                    if (fqstate.Board[x, y] == fqstate.SeasonIndex + opAdd)
                    {
                        OpponentRowsCounts[y]++;
                        OpponentColsCounts[x]++;
                    }
                }
            }
        }
    }
}
