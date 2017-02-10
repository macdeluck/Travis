using System;
using System.Collections.Generic;
using Travis.Logic.Extensions;
using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// Represents <see cref="MultipleTicTacToe"/> game state.
    /// </summary>
    public partial class MultipleTicTacToeState : IState
    {
        /// <summary>
        /// Returns true if state is terminal.
        /// </summary>
        public bool IsTerminal => CheckWinningBoardState();

        /// <summary>
        /// Gets <see cref="MultipleTicTacToe"/> player to move.
        /// </summary>
        public MTTTPlayer CurrentPlayer { get; private set; }

        /// <summary>
        /// Retunrs identifier of current player.
        /// </summary>
        public int CurrentPlayerId => CurrentPlayer == MTTTPlayer.XPlayer ? 0 : 1;

        /// <summary>
        /// Creates new <see cref="MultipleTicTacToe"/> game state.
        /// </summary>
        /// <param name="boardData">State board data.</param>
        /// <param name="currentPlayer"><see cref="MultipleTicTacToe"/> game player to move.</param>
        public MultipleTicTacToeState(MTTTPlayer[,] boardData, MTTTPlayer currentPlayer)
        {
            BoardData = boardData;
            CurrentPlayer = currentPlayer;
            ClearWinningBoardData();
            ClearActions();
        }

        /// <summary>
        /// Creates initial <see cref="MultipleTicTacToe"/> game state.
        /// </summary>
        public MultipleTicTacToeState()
            : this(new MTTTPlayer[Length, Length].Fill(MTTTPlayer.None), MTTTPlayer.XPlayer)
        {
        }

        /// <summary>
        /// Applies actions to state.
        /// </summary>
        /// <param name="actionSet"></param>
        public void Apply(ActionSet actionSet)
        {
            var currentPlayerAction = actionSet.Actions[CurrentPlayerId] as MultipleTicTacToeAction;
            SetBoardField(currentPlayerAction.BoardNum, currentPlayerAction.XPosition, currentPlayerAction.YPosition, CurrentPlayer);
            CurrentPlayer = CurrentPlayer.Opponent();
            ClearWinningBoardData();
            ClearActions();
        }

        /// <summary>
        /// Clones current state.
        /// </summary>
        public IState Clone()
        {
            return new MultipleTicTacToeState(BoardData.CloneArray(), CurrentPlayer);
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
                ActionSetId = actions[CurrentPlayerId].ActionId
            };
        }

        /// <summary>
        /// Returns available actions to take by actor.
        /// <param name="actorId">Actor identifier.</param>
        /// </summary>
        /// <returns>Dictionary of actions keyed with theirs identifiers.</returns>
        public IDictionary<int, IAction> GetActionsForActor(int actorId)
        {
            InitActions();
            return actorId == CurrentPlayerId ? _actionsForCurrentActor : new Dictionary<int, IAction>()
            {
                { 0, new MultipleTicTacToeAction() { ActionId = 0, ActorId = actorId, IsNoop = true } }
            };
        }

        private IDictionary<int, IAction> _actionsForCurrentActor;

        private IDictionary<int, IAction> InitActions()
        {
            if (_actionsForCurrentActor == null)
            {
                _actionsForCurrentActor = new Dictionary<int, IAction>();
                int num = 0;
                foreach (var board in _boards)
                    for (int x = 0; x < Span; x++)
                        for (int y = 0; y < Span; y++)
                        {
                            if (board[x, y] == MTTTPlayer.None)
                            {
                                var newAction = new MultipleTicTacToeAction()
                                {
                                    ActionId = num++,
                                    ActorId = CurrentPlayerId,
                                    BoardNum = board.Number,
                                    XPosition = x,
                                    YPosition = y
                                };
                                _actionsForCurrentActor.Add(newAction.ActionId, newAction);
                            }
                        }
            }
            return _actionsForCurrentActor;
        }

        private void ClearActions()
        {
            _actionsForCurrentActor = null;
        }

        /// <summary>
        /// Gets payoffs for terminal state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Should be thrown when method called on non terminal state.</exception>
        public IDictionary<int, double> GetPayoffs()
        {
            if (!IsTerminal)
                throw new InvalidOperationException();
            if (_winningPlayer == MTTTPlayer.None)
                return new Dictionary<int, double>() { { 0, 0.5 }, { 1, 0.5 } };
            else if (_winningPlayer == MTTTPlayer.XPlayer)
                return new Dictionary<int, double>() { { 0, 1 }, { 1, 0 } };
            else return new Dictionary<int, double>() { { 0, 0 }, { 1, 1 } };
        }
    }
}
