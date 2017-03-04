using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Games.MultipleTicTacToe;
using Travis.Logic.Model;

namespace Travis.Test.Games
{
    [TestClass]
    public class MultipleTicTacToeTest
    {
        [TestMethod]
        public void MTTTStateTransitionTest()
        {
            var game = new MultipleTicTacToe();
            var state = game.GetInitialState() as MultipleTicTacToeState;
            Assert.AreEqual(TicTacToeEntity.X, state.ControlTicTacToePlayer);
            Assert.AreEqual(0, state.ControlPlayer);
            CustomAssert.AssertState(state);

            var actions = state.GetActionsForActor(0).Values.OfType<MultipleTicTacToeAction>()
                .Where(a => a.BoardNum == 4 && a.PosX == 2 && a.PosY == 1).ToList();
            Assert.AreEqual(1, actions.Count);
            var stateClone = state.Clone() as MultipleTicTacToeState;
            CustomAssert.AssertState(stateClone);

            var aset = state.CreateActionSet(new Dictionary<int, IAction>()
            {
                { 0, actions.Single() },
                { 1, state.GetActionsForActor(1).Values.Single() }
            });
            state.Apply(aset);
            CustomAssert.AssertState(stateClone);
            CustomAssert.AssertState(state, new Dictionary<Tuple<int, int, int>, TicTacToeEntity>()
            {
                { Tuple.Create(4, 2, 1), TicTacToeEntity.X }
            });
            Assert.AreEqual(TicTacToeEntity.X, stateClone.ControlTicTacToePlayer);
            Assert.AreEqual(0, stateClone.ControlPlayer);

            Assert.AreEqual(TicTacToeEntity.O, state.ControlTicTacToePlayer);
            Assert.AreEqual(1, state.ControlPlayer);
        }

        [TestMethod]
        public void MTTTNoWinnerTest()
        {
            var board = new TicTacToeBoard(9);
            board[1, 1] = TicTacToeEntity.X;
            board[0, 0] = TicTacToeEntity.O;
            board[2, 0] = TicTacToeEntity.X;
            board[0, 2] = TicTacToeEntity.O;
            board[0, 1] = TicTacToeEntity.X;
            board[2, 1] = TicTacToeEntity.O;
            board[2, 2] = TicTacToeEntity.X;
            board[1, 2] = TicTacToeEntity.O;
            board[1, 0] = TicTacToeEntity.X;
            Assert.AreEqual(TicTacToeEntity.None, board.Winner);
        }
    }
}
