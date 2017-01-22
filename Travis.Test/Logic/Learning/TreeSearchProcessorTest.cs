using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Travis.Games.GreedyNumbers;
using Travis.Logic.Learning;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using System;
using System.Linq;
using Travis.Logic.Model;

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
            AssertTreeNumVisited(tree, iterations);
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
            AssertTreeNumVisited(tree, iterations);
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
            AssertTreeNumVisited(tree, iterations);

            var newIterations = 999;
            processor.Process(tree, game, newIterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations + newIterations, tree.Quality.NumVisited);
            AssertTreeNumVisited(tree, iterations + newIterations);
        }

        private void AssertTreeNumVisited(TreeNode node, int iterations, int depth = 0)
        {
            if (node.IsTerminal)
            {
                Assert.IsFalse(node.Children.Any());
                Assert.IsFalse(node.Quality.ActorActionsQualities.Any());
                return;
            }

            // Sum of children num visited is equal to parent node num visited decremented by one.
            // The exception is for root node, because root node is initially not visited.
            Assert.AreEqual(iterations - (depth == 0 ? 0 : 1), node.Children.Sum(ch => ch.Value.Quality.NumVisited), "Incorrect at depth {0}", depth);

            foreach (var actorActionsQuality in node.Quality.ActorActionsQualities.Values)
            {
                // Action to appear in quality info, must be visited at least once.
                Assert.IsFalse(actorActionsQuality.Values.Any(q => q.NumSelected == 0));

                // Sum of num visited for actor actions must be equal to node's num visited.
                Assert.AreEqual(node.Quality.NumVisited, actorActionsQuality.Values.Sum(q => q.NumSelected));
            }

            // Assert child node.
            foreach (var childNode in node.Children.Values)
            {
                AssertTreeNumVisited(childNode, childNode.Quality.NumVisited, depth + 1);
            }
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
            var topActionQuality = currentActorQualities.ActionQuality(topAction.ActionId);
            foreach (var aq in currentActorQualities)
            {
                if (aq.Key != topAction.ActionId)
                {
                    Assert.IsTrue(aq.Value.NumSelected < topActionQuality.NumSelected);
                    Assert.IsTrue(aq.Value.Quality < topActionQuality.Quality);
                }
            }
        }
    }
}
