using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Travis.Logic.Extensions;
using Travis.Logic.Model;

namespace Travis.Games.FarmingQuandaries
{
    /// <summary>
    /// Season utils.
    /// </summary>
    public static class FarmingQuandariesSeason
    {
        /// <summary>
        /// Winer season constant.
        /// </summary>
        public const int Winter = 0;

        /// <summary>
        /// Spring season constant.
        /// </summary>
        public const int Spring = 1;

        /// <summary>
        /// Summer season constant.
        /// </summary>
        public const int Summer = 2;

        /// <summary>
        /// Fall season constant.
        /// </summary>
        public const int Fall = 3;

        /// <summary>
        /// Converts farming quandaries season to string.
        /// </summary>
        /// <param name="season">Number of season.</param>
        public static string AsString(int season)
        {
            return SEASON_NAMES[season];
        }

        /// <summary>
        /// Names of seasons.
        /// </summary>
        public static readonly string[] SEASON_NAMES = new[] { "Winter", "Spring", "Summer", "Fall" };
    }

    /// <summary>
    /// Represents <see cref="FarmingQuandaries"/> game state.
    /// </summary>
    public class FarmingQuandariesState : IState
    {
        /// <summary>
        /// State board.
        /// </summary>
        public int[,] Board { get; private set; }

        /// <summary>
        /// Player points.
        /// </summary>
        public int[] Points { get; private set; }

        /// <summary>
        /// Game step.
        /// </summary>
        public int GameStep { get; private set; }

        /// <summary>
        /// Information if player has arson.
        /// </summary>
        public bool[] HasArson { get; private set; }

        private IDictionary<int, IAction>[] _legalActions;
        private int[] _legalActionsCounts;

        /// <summary>
        /// The player who choses main action.
        /// </summary>
        public int ControlPlayer { get; private set; }

        /// <summary>
        /// Checks if game state is terminal.
        /// </summary>
        public bool IsTerminal => GameStep >= 80;

        /// <summary>
        /// Creates new instance of class.
        /// </summary>
        /// <param name="controlPlayer">Id of player with control.</param>
        /// <param name="board">Game board.</param>
        /// <param name="points">Points gained by platers.</param>
        /// <param name="hasArson">Information if player has arson.</param>
        /// <param name="gameStep">Current game step.</param>
        public FarmingQuandariesState(int controlPlayer, int[,] board, int[] points, bool[] hasArson, int gameStep)
        {
            ControlPlayer = controlPlayer;
            Board = board;
            Points = points;
            HasArson = hasArson;
            GameStep = gameStep;
            InitLegalActions();
        }

        /// <summary>
        /// Starting player in given state.
        /// </summary>
        public int YearStartPlayer => (GameStep / 8) % 2;

        /// <summary>
        /// Returns season index.
        /// </summary>
        public int SeasonIndex => (GameStep % 8) / 2;

        /// <summary>
        /// Returns current season name.
        /// </summary>
        public string CurrentSeason => FarmingQuandariesSeason.AsString(SeasonIndex);

        /// <summary>
        /// Returns string representation of state.
        /// </summary>
        public string[] Serialize()
        {
            var lines = new List<string>();
            lines.Add("Control player: " + ControlPlayer.ToString());
            lines.Add("Year start player: " + YearStartPlayer.ToString());
            lines.Add("Game step: " + GameStep.ToString());
            lines.Add("Current season: " + CurrentSeason);
            lines.Add("Player 0 points: " + Points[0].ToString());
            lines.Add("Player 1 points: " + Points[1].ToString());
            lines.Add("Player 0 has arson: " + HasArson[0].ToString());
            lines.Add("Player 1 has arson: " + HasArson[1].ToString());
            lines.Add("  " + string.Join(" ", Enumerable.Range(1, Board.GetLength(0))));
            for (int y = 0; y < Board.GetLength(1); y++)
            {
                var sb = new StringBuilder();
                sb.Append(y + 1);
                sb.Append(' ');
                for (int x = 0; x < Board.GetLength(0); x++)
                {
                    sb.Append(Board[x, y]);
                    sb.Append(' ');
                }
                lines.Add(sb.ToString());
            }
            return lines.ToArray();
        }

        /// <summary>
        /// Clones itself.
        /// </summary>
        public IState Clone()
        {
            return new FarmingQuandariesState(ControlPlayer, Board.CloneArray(), Points.CloneArray(), HasArson.CloneArray(), GameStep);
        }

        private void InitLegalActions()
        {
            _legalActions = new Dictionary<int, IAction>[2];
            _legalActions[ControlPlayer] = InitControlPlayerActions();
            _legalActions[1 - ControlPlayer] = InitNotControlPlayerActions();
            _legalActionsCounts = new int[2];
            _legalActionsCounts[ControlPlayer] = 8;
            _legalActionsCounts[1 - ControlPlayer] = HasArson[1 - ControlPlayer] ? 9 : 1;
        }

        private Dictionary<int, IAction> InitControlPlayerActions()
        {
            var actions = new Dictionary<int, IAction>();
            int id = 0;
            for (int x = 0; x < Board.GetLength(0); x++, id++)
            {
                actions.Add(id, new FarmingQuandariesAction()
                {
                    ActorId = ControlPlayer,
                    ActionId = id,
                    IsRowAction = true,
                    Index = x,
                    IsNoop = false,
                    FarmingAction = FarmingActions.FromSeason(SeasonIndex)
                });
            }
            for (int y = 0; y < Board.GetLength(1); y++, id++)
            {
                actions.Add(id, new FarmingQuandariesAction()
                {
                    ActorId = ControlPlayer,
                    ActionId = id,
                    IsRowAction = false,
                    Index = y,
                    IsNoop = false,
                    FarmingAction = FarmingActions.FromSeason(SeasonIndex)
                });
            }
            return actions;
        }

        private Dictionary<int, IAction> InitNotControlPlayerActions()
        {
            var actions = new Dictionary<int, IAction>();
            int id = 0;
            actions.Add(id, new FarmingQuandariesAction
            {
                ActionId = id,
                ActorId = 1 - ControlPlayer,
                IsNoop = true
            });
            id++;
            if (HasArson[1 - ControlPlayer])
            {
                for (int x = 0; x < Board.GetLength(0); x++, id++)
                    actions.Add(id, new FarmingQuandariesAction()
                    {
                        ActorId = 1 - ControlPlayer,
                        ActionId = id,
                        Index = x,
                        IsRowAction = true,
                        IsNoop = false,
                        FarmingAction = FarmingAction.Arson
                    });
                for (int y = 0; y < Board.GetLength(1); y++, id++)
                    actions.Add(id, new FarmingQuandariesAction()
                    {
                        ActorId = 1 - ControlPlayer,
                        ActionId = id,
                        Index = y,
                        IsRowAction = false,
                        IsNoop = false,
                        FarmingAction = FarmingAction.Arson
                    });
            }
            return actions;
        }
        
        private void CalculateBoardChanges(ActionSet actionSet)
        {
            int dim;
            var fAction = actionSet.Actions[ControlPlayer] as FarmingQuandariesAction;
            var sAction = actionSet.Actions[1 - ControlPlayer] as FarmingQuandariesAction;
            if (!sAction.IsNoop && sAction.FarmingAction == FarmingAction.Arson && HasArson[1 - ControlPlayer])
            {
                HasArson[1 - ControlPlayer] = false;
                dim = sAction.IsRowAction ? 0 : 1;
                for (int i = 0; i < Board.GetLength(dim); i++)
                {
                    var x = sAction.IsRowAction ? i : sAction.Index;
                    var y = sAction.IsRowAction ? sAction.Index : i;
                    Board[x, y] = 0;
                }
            }
            dim = fAction.IsRowAction ? 0 : 1;
            int sumHarvested = 0;
            var classicActions = new[] { FarmingAction.Plow, FarmingAction.Sow, FarmingAction.Water };
            for (int i = 0; i < Board.GetLength(dim); i++)
            {
                var x = fAction.IsRowAction ? i : fAction.Index;
                var y = fAction.IsRowAction ? fAction.Index : i;
                if (classicActions.Contains(fAction.FarmingAction) && Board[x, y] == (int)fAction.FarmingAction)
                    Board[x, y]++;
                else if (fAction.FarmingAction == FarmingAction.Harvest && Board[x, y] == (int)fAction.FarmingAction)
                {
                    sumHarvested++;
                    Board[x, y] = 0;
                }
            }
            if (!sAction.IsNoop && sAction.FarmingAction == FarmingAction.Arson && HasArson[1 - ControlPlayer])
            {
                HasArson[1 - ControlPlayer] = false;
                dim = sAction.IsRowAction ? 0 : 1;
                for (int i = 0; i < Board.GetLength(dim); i++)
                {
                    var x = sAction.IsRowAction ? i : sAction.Index;
                    var y = sAction.IsRowAction ? sAction.Index : i;
                    Board[x, y] = 0;
                }
            }
            if (sumHarvested >= 3)
                Points[ControlPlayer] += 10;
        }
        
        /// <summary>
        /// Returns string representation of state.
        /// </summary>
        public override string ToString()
        {
            return string.Join("\n", Serialize());
        }

        /// <summary>
        /// Returns available actions to take by actor.
        /// <param name="actorId">Actor identifier.</param>
        /// </summary>
        /// <returns>Dictionary of actions keyed with theirs identifiers.</returns>
        public IDictionary<int, IAction> GetActionsForActor(int actorId)
        {
            return _legalActions[actorId];
        }

        /// <summary>
        /// Applies action set to state and switches to next state.
        /// </summary>
        /// <param name="actionSet">A set of actions taken by actors.</param>
        public void Apply(ActionSet actionSet)
        {
            CalculateBoardChanges(actionSet);
            if (GameStep % 8 != 7)
            {
                ControlPlayer = 1 - ControlPlayer;
            }
            else
                Board = new int[Board.GetLength(0), Board.GetLength(1)];
            GameStep++;
            if (GameStep % 16 == 0)
                for (int i = 0; i < HasArson.Length; i++)
                    HasArson[i] = true;
            InitLegalActions();
        }

        /// <summary>
        /// Creates action set from chosen actions.
        /// </summary>
        /// <param name="actions">Actions chosen by actors keyed with their ids.</param>
        public ActionSet CreateActionSet(IDictionary<int, IAction> actions)
        {
            return new ActionSet()
            {
                Actions = actions,
                ActionSetId = actions[ControlPlayer].ActionId * (_legalActionsCounts[1 - ControlPlayer]) + actions[1 - ControlPlayer].ActionId
            };
        }

        /// <summary>
        /// Gets payoffs for terminal state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when method called on non terminal state.</exception>
        public IDictionary<int, double> GetPayoffs()
        {
            if (!IsTerminal) throw new InvalidOperationException("Game is not in terminal state.");
            if (Points[0] == Points[1])
                return new Dictionary<int, double> { { 0, 0.5 }, { 1, 0.5 } };
            if (Points[0] > Points[1])
                return new Dictionary<int, double> { { 0, 1.0 }, { 1, 0.0 } };
            return new Dictionary<int, double> { { 0, 0.0 }, { 1, 1.0 } };
        }
    }

}
