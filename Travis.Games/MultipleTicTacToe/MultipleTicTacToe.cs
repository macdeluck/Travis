using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// Represents Tic-Tac-Toe like game.
    /// </summary>
    public class MultipleTicTacToe : IGame
    {
        /// <summary>
        /// Gets game name.
        /// </summary>
        public string Name => nameof(MultipleTicTacToe);

        /// <summary>
        /// Returs number of actors for particular game.
        /// </summary>
        public int NumberOfActors => 2;

        private static TicTacToeBoard[] CreateInitBoards()
        {
            var arr = new TicTacToeBoard[MultipleTicTacToeState.BoardsNum];
            for (int i = 0; i < MultipleTicTacToeState.BoardsNum; i++)
                arr[i] = new TicTacToeBoard(MultipleTicTacToeState.BoardSize);
            return arr;
        }

        /// <summary>
        /// Enumerates actors Ids.
        /// </summary>
        public IEnumerable<int> EnumerateActors()
        {
            return Enumerable.Range(0, NumberOfActors);
        }

        /// <summary>
        /// Returns initial state for game.
        /// </summary>
        public IState GetInitialState()
        {
            return new MultipleTicTacToeState(0, CreateInitBoards());
        }
    }
}
