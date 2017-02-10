using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// A Tic-Tac-Toe like game with few boards and only one winning.
    /// </summary>
    public class MultipleTicTacToe : IGame
    {
        /// <summary>
        /// Gets game name.
        /// </summary>
        public string Name => nameof(MultipleTicTacToe);

        /// <summary>
        /// Gets number of players.
        /// </summary>
        public int NumberOfActors => 2;

        /// <summary>
        /// Enumerates player's identifiers.
        /// </summary>
        public IEnumerable<int> EnumerateActors() => Enumerable.Range(0, NumberOfActors);

        /// <summary>
        /// Returns initial game state.
        /// </summary>
        public IState GetInitialState()
        {
            return new MultipleTicTacToeState();
        }
    }
}
