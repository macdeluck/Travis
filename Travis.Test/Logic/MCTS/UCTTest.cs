using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Travis.Games.GreedyNumbers;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Test.Logic.MCTS
{
    [TestClass]
    public class UCTTest
    {
        [TestMethod]
        public void UCT_NotFullyExpanded()
        {
            var node = new TreeNode();
            node.Quality.NumVisited = 2;
            node.Quality.ActorActionsQualities[0] = new ActorQualityInfo()
            {
                { 0, new ActionQualityInfo() { Quality = 0.5, NumSelected = 1 } },
                { 1, new ActionQualityInfo() { Quality = 0, NumSelected = 1 } }
            };
            var state = new Mock<IState>();
            state.Setup(s => s.GetActionsForActor(It.IsAny<int>())).Returns(() => new Dictionary<int, IAction>()
            {
                { 0, new GreedyNumbersAction() { ActionId = 0, ActorId = 0 } },
                { 1, new GreedyNumbersAction() { ActionId = 1, ActorId = 0 } },
                { 2, new GreedyNumbersAction() { ActionId = 2, ActorId = 0 } }
            });
            var uct = new UCT();
            var action = uct.Invoke(node, state.Object, 0);
            Assert.AreEqual(0, action.ActorId);
            Assert.AreEqual(2, action.ActionId);
        }

        [TestMethod]
        public void UCT_FullyExpanded_BestActionChosen()
        {
            var node = new TreeNode();
            node.Quality.NumVisited = 15;
            node.Quality.ActorActionsQualities[0] = new ActorQualityInfo()
            {
                { 0, new ActionQualityInfo() { Quality = 0.2, NumSelected = 4 } },
                { 1, new ActionQualityInfo() { Quality = 0.7, NumSelected = 6 } },
                { 2, new ActionQualityInfo() { Quality = 0.1, NumSelected = 5 } }
            };
            var state = new Mock<IState>();
            state.Setup(s => s.GetActionsForActor(It.IsAny<int>())).Returns(() => new Dictionary<int, IAction>()
            {
                { 0, new GreedyNumbersAction() { ActionId = 0, ActorId = 0 } },
                { 1, new GreedyNumbersAction() { ActionId = 1, ActorId = 0 } },
                { 2, new GreedyNumbersAction() { ActionId = 2, ActorId = 0 } }
            });
            var uct = new UCT();
            var action = uct.Invoke(node, state.Object, 0);
            Assert.AreEqual(0, action.ActorId);
            Assert.AreEqual(1, action.ActionId);
        }

        [TestMethod]
        public void UCT_FullyExpanded_WorstActionChosen()
        {
            var node = new TreeNode();
            node.Quality.NumVisited = 15;
            node.Quality.ActorActionsQualities[0] = new ActorQualityInfo()
            {
                { 0, new ActionQualityInfo() { Quality = 0.2, NumSelected = 6 } },
                { 1, new ActionQualityInfo() { Quality = 0.7, NumSelected = 8 } },
                { 2, new ActionQualityInfo() { Quality = 0.1, NumSelected = 1 } }
            };
            var state = new Mock<IState>();
            state.Setup(s => s.GetActionsForActor(It.IsAny<int>())).Returns(() => new Dictionary<int, IAction>()
            {
                { 0, new GreedyNumbersAction() { ActionId = 0, ActorId = 0 } },
                { 1, new GreedyNumbersAction() { ActionId = 1, ActorId = 0 } },
                { 2, new GreedyNumbersAction() { ActionId = 2, ActorId = 0 } }
            });
            var uct = new UCT();
            var action = uct.Invoke(node, state.Object, 0);
            Assert.AreEqual(0, action.ActorId);
            Assert.AreEqual(2, action.ActionId);
        }

        [TestMethod]
        public void UCT_CalculateValue()
        {
            Assert.AreEqual(0.3, UCT.CalculateUCT(1, 0.3, 0, 5));
            Assert.AreEqual(1.2718, Math.Round(UCT.CalculateUCT(1, 0.6, 6, 15), 4));
            Assert.AreEqual(1.07225, Math.Round(UCT.CalculateUCT(1.2, 0.3, 6, 12), 5));
        }
    }
}
