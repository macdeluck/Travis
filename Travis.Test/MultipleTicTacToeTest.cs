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
            Assert.AreEqual(MTTTPlayer.XPlayer, state.CurrentPlayer);
            Assert.AreEqual(0, state.CurrentPlayerId);
            CustomAssert.AssertState(state);

            var actions = state.GetActionsForActor(0).Values.OfType<MultipleTicTacToeAction>()
                .Where(a => a.BoardNum == 4 && a.XPosition == 2 && a.YPosition == 1).ToList();
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
            CustomAssert.AssertState(state, new Dictionary<System.Tuple<int, int, int>, MTTTPlayer>()
            {
                { Tuple.Create(4, 2, 1), MTTTPlayer.XPlayer }
            });
            Assert.AreEqual(MTTTPlayer.XPlayer, stateClone.CurrentPlayer);
            Assert.AreEqual(0, stateClone.CurrentPlayerId);

            Assert.AreEqual(MTTTPlayer.YPlayer, state.CurrentPlayer);
            Assert.AreEqual(1, state.CurrentPlayerId);
        }
    }
}
