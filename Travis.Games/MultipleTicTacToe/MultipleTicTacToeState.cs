using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// Represents <see cref="MultipleTicTacToe"/> game state.
    /// </summary>
    public class MultipleTicTacToeState : IState
    {
        /// <summary>
        /// Game boards.
        /// </summary>
        public TicTacToeBoard[] Boards { get; private set; }

        /// <summary>
        /// Size of single board.
        /// </summary>
        public const int BoardSize = 3;

        /// <summary>
        /// Number of boards.
        /// </summary>
        public const int BoardsNum = 9;

        /// <summary>
        /// The winning board.
        /// </summary>
        public const int WinningBoard = 4;

        /// <summary>
        /// Identifier of player who has control.
        /// </summary>
        public int ControlPlayer { get; private set; }

        private IDictionary<int, IAction>[] _actions;

        /// <summary>
        /// Returns player who has control on board.
        /// </summary>
        public TicTacToeEntity ControlTicTacToePlayer
        {
            get
            {
                if (ControlPlayer == 0)
                    return TicTacToeEntity.X;
                if (ControlPlayer == 1)
                    return TicTacToeEntity.O;
                return TicTacToeEntity.None;
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="MultipleTicTacToeState"/>.
        /// </summary>
        /// <param name="controlPlayer">Player which has current control on game.</param>
        /// <param name="boards">Game boards.</param>
        public MultipleTicTacToeState(int controlPlayer, TicTacToeBoard[] boards)
        {
            Boards = boards;
            ControlPlayer = controlPlayer;
            InitLegalActions();
        }

        private void InitLegalActions()
        {
            _actions = new Dictionary<int, IAction>[2];
            _actions[1 - ControlPlayer] = new Dictionary<int, IAction>
            {
                { 0, new MultipleTicTacToeAction() {
                        ActionId = 0,
                        ActorId = 1 - ControlPlayer,
                        IsNoop = true
                    }
                }
            };
            InitControlActions();
        }

        private void InitControlActions()
        {
            int currentId = 0;
            var controlActions = new Dictionary<int, IAction>(BoardSize * BoardSize * BoardsNum);
            for (int boardNum = 0; boardNum < Boards.Length; boardNum++)
                AppendActionsForBoard(boardNum, controlActions, ref currentId);
            _actions[ControlPlayer] = controlActions;
        }

        private void AppendActionsForBoard(int boardNum, Dictionary<int, IAction> controlActions, ref int currentId)
        {
            var board = Boards[boardNum];
            if (board.Winner != null) return;
            for (int x = 0; x < board.Size; x++)
                for (int y = 0; y < board.Size; y++)
                    if (board[x, y] == TicTacToeEntity.None)
                    {
                        var id = currentId++;
                        controlActions.Add(id, new MultipleTicTacToeAction()
                        {
                            ActionId = id,
                            ActorId = ControlPlayer,
                            BoardNum = boardNum,
                            PosX = x,
                            PosY = y
                        });
                    }
        }

        /// <summary>
        /// Checks if current state is terminal.
        /// </summary>
        public bool IsTerminal => Boards[WinningBoard].Winner != null;

        /// <summary>
        /// Serializes state to its string representation.
        /// </summary>
        public string[] Serialize()
        {
            var result = new List<string>(12);
            result.Add("Control player: {0}".FormatString(ControlTicTacToePlayer));
            result.Add(string.Empty);
            var boardRows = BoardsNum / 3;
            for (int i = 0; i < boardRows; i++)
            {
                AppendBoardRow(result, Boards[i * boardRows], Boards[i * boardRows + 1], Boards[i * boardRows + 2]);
                if (i != boardRows - 1)
                    AppendSeparator(result);
            }
            return result.ToArray();
        }

        private void AppendBoardRow(List<string> result, params TicTacToeBoard[] boards)
        {
            var boardStrings = boards.Select(b => b.Serialized).ToArray();
            for (int rowNum = 0; rowNum < boardStrings.First().Length; rowNum++)
                result.Add(string.Join("|", Enumerable.Range(0, boardStrings.Length).Select(boardnum => boardStrings[boardnum][rowNum])));
        }

        private void AppendSeparator(List<string> result)
        {
            result.Add(new string(Enumerable.Repeat('-', 6 * 3).ToArray()));
        }

        /// <summary>
        /// Clones itself.
        /// </summary>
        public IState Clone()
        {
            return new MultipleTicTacToeState(ControlPlayer, Boards.CloneArray(b => b.Clone()));
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
        /// Applies action set to state and switches to next state.
        /// </summary>
        /// <param name="actionSet">A set of actions taken by actors.</param>
        public void Apply(ActionSet actionSet)
        {
            var action = actionSet.Actions[ControlPlayer] as MultipleTicTacToeAction;
            Boards[action.BoardNum][action.PosX, action.PosY] = ControlTicTacToePlayer;
            ControlPlayer = 1 - ControlPlayer;
            InitLegalActions();
        }

        /// <summary>
        /// Returns string representation of game state.
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
            return _actions[actorId];
        }

        /// <summary>
        /// Gets payoffs for terminal state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when method called on non terminal state.</exception>
        public IDictionary<int, double> GetPayoffs()
        {
            if (!IsTerminal) throw new InvalidOperationException();
            if (Boards[WinningBoard].Winner != TicTacToeEntity.None)
            {
                if (Boards[WinningBoard].Winner == TicTacToeEntity.X)
                    return new Dictionary<int, double>() { { 0, 1.0 }, { 1, 0.0 } };
                return new Dictionary<int, double>() { { 0, 0.0 }, { 1, 1.0 } };
            }
            return new Dictionary<int, double>() { { 0, 0.5 }, { 1, 0.5 } };
        }
    }
}
