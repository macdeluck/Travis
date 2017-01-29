using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Travis.Games.GreedyNumbers;
using Travis.Logic.Learning;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using System.Linq;

namespace Travis.Test.Logic.Learning
{
    [TestClass]
    public class TreeSearchProcessorTest
    {
        /// <summary>
        /// Checks num visited of not fully expanded tree.
        /// </summary>
        [TestMethod]
        public void NumVisitedTest()
        {
            var processor = new TreeSearchProcessor();
            var iterations = 100;
            var tree = new TreeNode();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 1000 }, { 2, 3 }, { 5, 12 }, { 12000, 1 } });
            processor.Process(tree, game, iterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations, tree.Quality.NumVisited);
            CustomAssert.AssertTree(tree, iterations);
        }

        /// <summary>
        /// Checks structure of tree which is a path.
        /// </summary>
        [TestMethod]
        public void NumVisitedTest_TreeIsAPath()
        {
            var processor = new TreeSearchProcessor();
            var iterations = 100;
            var tree = new TreeNode();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 3 } });
            processor.Process(tree, game, iterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations, tree.Quality.NumVisited);
            CustomAssert.AssertTree(tree, iterations);
        }

        /// <summary>
        /// Checks structure of tree which is initially fully expanded.
        /// </summary>
        [TestMethod]
        public void NumVisitedTest_SingleIterationFullyExpands()
        {
            var processor = new TreeSearchProcessor();
            var iterations = 1;
            var tree = new TreeNode();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 1 } });
            processor.Process(tree, game, iterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations, tree.Quality.NumVisited);
            CustomAssert.AssertTree(tree, iterations);

            var newIterations = 999;
            processor.Process(tree, game, newIterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations + newIterations, tree.Quality.NumVisited);
            CustomAssert.AssertTree(tree, iterations + newIterations);
        }

        /// <summary>
        /// Checks if top action has best quality and is is mostly selected.
        /// </summary>
        [TestMethod]
        public void QualityTest_BestActionWithGreatesValue()
        {
            var processor = new TreeSearchProcessor();
            var iterations = 1000;
            var tree = new TreeNode();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 5 }, { 2, 3 }, { 7, 1 } });
            processor.Process(tree, game, iterations, MCTSActionSelector.Create(game.EnumerateActors()));
            var beginState = game.GetInitialState() as GreedyNumbersState;
            var topAction = beginState.GetActionsForActor(beginState.CurrentActorId).Values.Cast<GreedyNumbersAction>().FirstOrDefault(a => a.PickValue == 7);
            var currentActorQualities = tree.Quality.ActorActionsQualities[topAction.ActorId];
            var topActionQuality = currentActorQualities[topAction.ActionId];
            foreach (var aq in currentActorQualities)
            {
                if (aq.Key != topAction.ActionId)
                {
                    Assert.IsTrue(aq.Value.NumSelected < topActionQuality.NumSelected);
                    Assert.IsTrue(aq.Value.Quality < topActionQuality.Quality);
                }
            }
        }

        [TestMethod]
        public void TreeSearch_EventsTest()
        {
            var processor = new TreeSearchProcessor();
            var iterations = 1;
            var tree = new TreeNode();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 5 }, { 2, 3 }, { 7, 1 } });
            var startCounter = 0;
            var finishCounter = 0;
            var cstate = game.GetInitialState() as GreedyNumbersState;
            processor.IterationStarted += (node, state) => 
            {
                startCounter++;
                var gstate = state as GreedyNumbersState;
                CustomAssert.AreEqual(cstate, gstate);
                Assert.IsFalse(tree.Children.Any());
                Assert.AreEqual(0, tree.Quality.NumVisited);
                Assert.IsFalse(tree.Quality.ActorActionsQualities.Any());
            };
            processor.StateTransition += (node, state, actions) =>
            {
                var gstate = state as GreedyNumbersState;
                CustomAssert.AreEqual(cstate, gstate);
                cstate.Apply(actions);
            };
            processor.IterationFinished += state =>
            {
                finishCounter++;
                var gstate = state as GreedyNumbersState;
                CustomAssert.AreEqual(cstate, gstate);
            };
            processor.Process(tree, game, iterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(1, startCounter);
            Assert.AreEqual(1, finishCounter);
            Assert.AreEqual(1, tree.Children.Count);
            Assert.AreEqual(1, tree.Quality.NumVisited);
            var cnode = tree.Children.Values.Single();
            Assert.AreEqual(0, cnode.Children.Count);
            Assert.AreEqual(1, cnode.Quality.NumVisited);
        }
    }
}
