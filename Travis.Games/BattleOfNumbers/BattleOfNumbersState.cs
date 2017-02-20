using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Travis.Logic.Model;

namespace Travis.Games.BattleOfNumbers
{
    /// <summary>
    /// Represents <see cref="BattleOfNumbers"/> game state.
    /// </summary>
    public class BattleOfNumbersState : IState
    {
        /// <summary>
        /// Returns state board.
        /// </summary>
        public int[,] Board { get; private set; }

        /// <summary>
        /// Returns current actor identifier.
        /// </summary>
        public int ControlPlayer { get; private set; }

        /// <summary>
        /// Player points.
        /// </summary>
        public int[] Points { get; private set; }

        /// <summary>
        /// Picks available for players.
        /// </summary>
        public IDictionary<int, int[]>[] Picks { get; private set; }

        private IDictionary<int, IAction>[] _legalActions;
        private int[] _legalActionsCounts;

        /// <summary>
        /// Creates new instance of state.
        /// </summary>
        /// <param name="controlPlayer">Control player.</param>
        /// <param name="picks">Player picks.</param>
        /// <param name="points">Player points.</param>
        public BattleOfNumbersState(int controlPlayer, IDictionary<int, int[]>[] picks, int[] points)
        {
            InitState(controlPlayer, picks, points);
        }

        private void InitState(int controlPlayer, IDictionary<int, int[]>[] picks, int[] points)
        {
            ControlPlayer = controlPlayer;
            Picks = picks;
            Points = points;
            InitBoard();
            InitLegalActions();
        }

        private void InitBoard()
        {
            Board = new int[5, 5];
            PlacePlayerPicks(Picks[0]);
            PlacePlayerPicks(Picks[1]);
        }

        private void PlacePlayerPicks(IDictionary<int, int[]> playerPicks)
        {
            foreach (var pick in playerPicks.Keys)
            {
                var pos = playerPicks[pick];
                Board[pos[0], pos[1]] = pick;
            }
        }

        private void InitLegalActions()
        {
            _legalActions = new Dictionary<int, IAction>[2];
            _legalActions[1 - ControlPlayer] = new Dictionary<int, IAction>() { { 0, new BattleOfNumbersAction() { ActionId = 0, ActorId = 1 - ControlPlayer, IsNoop = true } } };
            var idGen = 0;
            _legalActions[ControlPlayer] = Picks[ControlPlayer].SelectMany(pick => LegalActionsFromPick(pick.Key, pick.Value, ref idGen)).ToDictionary(p => p.ActionId, p => p);
            if (_legalActions[ControlPlayer].Count == 0)
                _legalActions[ControlPlayer] = new Dictionary<int, IAction>() { { 0, new BattleOfNumbersAction() { ActionId = 0, ActorId = ControlPlayer, IsNoop = true } } };
            _legalActionsCounts = _legalActions.Select(a => a.Count).ToArray();
        }

        private IEnumerable<IAction> LegalActionsFromPick(int pick, int[] position, ref int idGen)
        {
            var intIdGen = idGen;
            var targetPositions = new List<int[]>();
            targetPositions.Add(new[] { position[0] - Math.Sign(pick), position[1] - 1 });
            targetPositions.Add(new[] { position[0] - Math.Sign(pick), position[1] });
            targetPositions.Add(new[] { position[0] - Math.Sign(pick), position[1] + 1 });
            var endPositions = targetPositions.Where(pos => CanMove(pick, position, pos));
            var result = endPositions.Select(pos => new BattleOfNumbersAction()
            {
                ActorId = ControlPlayer,
                ActionId = intIdGen++,
                IsNoop = false,
                Pick = pick,
                BeginPos = new[] { position[0], position[1] },
                EndPos = pos
            }).ToList();
            idGen = intIdGen;
            return result;
        }
        
        /// <summary>
        /// Checks if state is terminal.
        /// </summary>
        public bool IsTerminal => 
            (_legalActions[0][0] as BattleOfNumbersAction).IsNoop && 
            (_legalActions[1][0] as BattleOfNumbersAction).IsNoop;

        /// <summary>
        /// Clones itself.
        /// </summary>
        public IState Clone()
        {
            return new BattleOfNumbersState(ControlPlayer, ClonePicks(Picks), new[] { Points[0], Points[1] });
        }

        private IDictionary<int, int[]>[] ClonePicks(IDictionary<int, int[]>[] _picks)
        {
            var result = new Dictionary<int, int[]>[2];
            result[0] = ClonePlayersPicks(_picks[0]);
            result[1] = ClonePlayersPicks(_picks[1]);
            return result;
        }

        private Dictionary<int, int[]> ClonePlayersPicks(IDictionary<int, int[]> picks)
        {
            var result = new Dictionary<int, int[]>();
            foreach (var key in picks.Keys)
            {
                var pos = picks[key];
                result.Add(key, new[] { pos[0], pos[1] });
            }
            return result;
        }

        /// <summary>
        /// Checks if <paramref name="attacker"/> pick may beat <paramref name="defender"/>.
        /// </summary>
        /// <param name="attacker">Attacker pick value.</param>
        /// <param name="defender">Deffender pick value.</param>
        public static bool CanBeat(int attacker, int defender)
        {
            attacker = Math.Abs(attacker);
            defender = Math.Abs(defender);
            if (attacker != 1 && attacker != 5)
            {
                return attacker >= defender;
            }
            if (attacker == 1)
            {
                return defender == 1 || defender == 5;
            }
            else // if (attacker == 5)
            {
                return defender > 1;
            }
        }

        /// <summary>
        /// Checks if pick may move from one position to another.
        /// </summary>
        /// <param name="pick">Value of pick to move.</param>
        /// <param name="beginPos">Start pick position.</param>
        /// <param name="endPos">End pick position.</param>
        private bool CanMove(int pick, int[] beginPos, int[] endPos)
        {
            return CanMove(pick, beginPos, endPos, Board);
        }

        /// <summary>
        /// Checks if pick may move from one position to another.
        /// </summary>
        /// <param name="pick">Value of pick to move.</param>
        /// <param name="beginPos">Start pick position.</param>
        /// <param name="endPos">End pick position.</param>
        /// <param name="board">Board to move on.</param>
        public static bool CanMove(int pick, int[] beginPos, int[] endPos, int[,] board)
        {
            // move toward enemy, check X constraint
            if (beginPos[0] - Math.Sign(pick) != endPos[0] || endPos[0] < 0 || endPos[0] >= 5)
                return false;
            // not so far, check Y constraint
            if (Math.Abs(beginPos[1] - endPos[1]) > 1 || endPos[1] < 0 || endPos[1] >= 5)
                return false;
            // if target cell is not occupied
            if (board[endPos[0], endPos[1]] == 0)
                return true;
            // cannot beat own pieces
            else if (Math.Sign(board[endPos[0], endPos[1]]) == Math.Sign(pick))
                return false;
            // move through corner and can beat
            if ((Math.Abs(beginPos[1] - endPos[1]) == 1) && CanBeat(pick, board[endPos[0], endPos[1]]))
                return true;
            return false;
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
            var controlA = actionSet.Actions[ControlPlayer] as BattleOfNumbersAction;
            Picks[ControlPlayer][controlA.Pick][0] = controlA.EndPos[0];
            Picks[ControlPlayer][controlA.Pick][1] = controlA.EndPos[1];
            var enemyPick = Board[controlA.EndPos[0], controlA.EndPos[1]];
            if (enemyPick != 0)
            {
                Picks[1 - ControlPlayer].Remove(enemyPick);
                Points[ControlPlayer] += 1;
            }
            InitState(1 - ControlPlayer, Picks, Points);
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
                ActionSetId = actions[ControlPlayer].ActionId
            };
        }

        /// <summary>
        /// Serializes state to string representation.
        /// </summary>
        public string[] Serialize()
        {
            var lines = new List<string>();
            lines.Add(string.Format("Control player: {0}", ControlPlayer));
            lines.Add(string.Format("Player 0 points: {0}", Points[0]));
            lines.Add(string.Format("Player 1 points: {0}", Points[1]));
            lines.Add("Board:");
            lines.Add(string.Format("   {0}", string.Join(" ", Enumerable.Range(0, 5).Select(v => string.Format("{0,-2}", v)))));
            for (int j = 0; j < 5; j++)
            {
                var sb = new StringBuilder(17);
                sb.Append(j);
                for (int i = 0; i < 5; i++)
                    if (Board[i, j] != 0)
                        sb.Append(string.Format(" {0, 2}", Board[i, j]));
                    else sb.Append("   ");
                lines.Add(sb.ToString());
            }
            return lines.ToArray();
        }
        
        /// <summary>
        /// Returns string representation of state.
        /// </summary>
        public override string ToString()
        {
            return string.Join("\n", Serialize());
        }

        /// <summary>
        /// Gets payoffs for terminal state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when method called on non terminal state.</exception>
        public IDictionary<int, double> GetPayoffs()
        {
            if (!IsTerminal) throw new InvalidOperationException();
            if (Points[0] > Points[1])
                return new Dictionary<int, double> { { 0, 1.0 }, { 1, 0.0 } };
            else if (Points[0] < Points[1])
                return new Dictionary<int, double> { { 0, 0.0 }, { 1, 1.0 } };
            else return new Dictionary<int, double> { { 0, 0.5 }, { 1, 0.5 } };
        }
    }
}
