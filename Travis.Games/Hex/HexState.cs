using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Algorithm;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using static Travis.Games.Hex.HexGraph;

namespace Travis.Games.Hex
{
    /// <summary>
    /// Represents <see cref="Hex"/> game state.
    /// </summary>
    public class HexState : IState
    {
        /// <summary>
        /// Stores state board.
        /// </summary>
        public HexBoard Board { get; private set; }

        /// <summary>
        /// Identifier of player to move.
        /// </summary>
        public int ControlPlayer { get; private set; }
        
        private IDictionary<int, IAction>[] _actions;
        
        /// <summary>
        /// Creates new instance of state.
        /// </summary>
        /// <param name="controlPlayer">Player with control.</param>
        /// <param name="board">State board.</param>
        public HexState(int controlPlayer, HexBoard board)
        {
            Board = board;
            ControlPlayer = controlPlayer;
            InitActions();
        }

        private void InitActions()
        {
            _actions = new Dictionary<int, IAction>[2];
            _actions[1 - ControlPlayer] = new Dictionary<int, IAction>() { { 0, new HexAction()
            {
                ActionId = 0,
                ActorId = 1 - ControlPlayer,
                IsNoop = true
            }}};
            InitControlActions();
        }

        private void InitControlActions()
        {
            var result = new Dictionary<int, IAction>();
            int id = 0;
            for (int x = 0; x < Board.Size; x++)
                for (int y = 0; y < Board.Size; y++)
                    if (Board[x, y] == HexEntity.Empty)
                    {
                        result.Add(id, new HexAction()
                        {
                            ActionId = id,
                            ActorId = ControlPlayer,
                            IsNoop = false,
                            X = x,
                            Y = y
                        });
                        id++;
                    }
            _actions[ControlPlayer] = result;
        }

        /// <summary>
        /// Checks if state is terminal.
        /// </summary>
        public bool IsTerminal => Board.Winner != HexEntity.Empty;
        
        /// <summary>
        /// Serializes graph to string representation.
        /// </summary>
        public string[] Serialize()
        {
            var result = new List<string>(Board.Size + 2);
            result.Add("Control player: {0}".FormatString(ControlPlayer == 0 ? "Red" : "Black"));
            result.AddRange(Board.Serialize());
            return result.ToArray();
        }

        /// <summary>
        /// Clones itself.
        /// </summary>
        public IState Clone()
        {
            return new HexState(ControlPlayer, Board.Clone());
        }

        /// <summary>
        /// Gets payoffs for terminal state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when method called on non terminal state.</exception>
        public IDictionary<int, double> GetPayoffs()
        {
            if (!IsTerminal) throw new InvalidOperationException("State is not terminal");
            if (Board.Winner == HexEntity.Red)
                return new Dictionary<int, double> { { 0, 1.0 }, { 1, 0.0 } };
            return new Dictionary<int, double> { { 0, 0.0 }, { 1, 1.0 } };
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
        /// Applies action set to state and switches to next state.
        /// </summary>
        /// <param name="actionSet">A set of actions taken by actors.</param>
        public void Apply(ActionSet actionSet)
        {
            var cac = actionSet.Actions[ControlPlayer] as HexAction;
            Board[cac.X, cac.Y] = ControlPlayer == 0 ? HexEntity.Red : HexEntity.Black;
            ControlPlayer = 1 - ControlPlayer;
            InitActions();
        }

        /// <summary>
        /// Creates action set from chosen actions.
        /// </summary>
        /// <param name="actions">Actions chosen by actors keyed with their ids.</param>
        public ActionSet CreateActionSet(IDictionary<int, IAction> actions)
        {
            return new ActionSet()
            {
                ActionSetId = actions[ControlPlayer].ActionId,
                Actions = actions
            };
        }

        /// <summary>
        /// Returns object string representation.
        /// </summary>
        public override string ToString()
        {
            return Serialize().JoinString("\n");
        }
    }

}
