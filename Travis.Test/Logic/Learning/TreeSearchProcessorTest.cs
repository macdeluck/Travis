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
        [TestMethod]
        public void NumVisitedTest()
        {
            var processor = new TreeSearchProcessor();
            var iterations = 1000;
            var tree = new TreeNode();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 5 }, { 2, 3 }, { 7, 1 } });
            processor.Process(tree, game, iterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations, tree.Quality.NumVisited);
            AssertActionQualities(tree);
        }

        [TestMethod]
        public void NumVisitedTest_TreeIsAPath()
        {
            var processor = new TreeSearchProcessor();
            var iterations = 100;
            var tree = new TreeNode();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 3 } });
            processor.Process(tree, game, iterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations, tree.Quality.NumVisited);
            var testNode = tree;
            int depth = -1;
            while (testNode != null)
            {
                Assert.AreEqual(iterations - (depth < 0 ? 0 : depth), testNode.Quality.NumVisited, "Incorrect at depth {0}", depth);
                testNode = testNode.Children.Values.SingleOrDefault();
                depth++;
            }
            AssertActionQualities(tree);
        }

        [TestMethod]
        public void NumVisitedTest_SingleIterationFullyExpands()
        {
            var processor = new TreeSearchProcessor();
            var iterations = 1;
            var tree = new TreeNode();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 1 } });
            processor.Process(tree, game, iterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations, tree.Quality.NumVisited);
            var testNode = tree;
            int depth = -1;
            while (testNode != null)
            {
                Assert.AreEqual(iterations - (depth < 0 ? 0 : depth), testNode.Quality.NumVisited, "Incorrect at depth {0}", depth);
                testNode = testNode.Children.Values.SingleOrDefault();
                depth++;
            }
            AssertActionQualities(tree);
            var newIterations = 999;
            processor.Process(tree, game, newIterations, MCTSActionSelector.Create(game.EnumerateActors()));
            Assert.AreEqual(iterations + newIterations, tree.Quality.NumVisited);
            testNode = tree;
            depth = -1;
            while (testNode != null)
            {
                Assert.AreEqual(iterations + newIterations - (depth < 0 ? 0 : depth), testNode.Quality.NumVisited, "Incorrect at depth {0}", depth);
                testNode = testNode.Children.Values.SingleOrDefault();
                depth++;
            }
            AssertActionQualities(tree);
        }

        private void AssertActionQualities(TreeNode node)
        {
            foreach (var actorActionsQuality in node.Quality.ActorActionsQualities.Values)
            {
                Assert.IsFalse(actorActionsQuality.Values.Any(q => q.NumSelected == 0));
                Assert.AreEqual(node.Quality.NumVisited, actorActionsQuality.Values.Sum(q => q.NumSelected));
            }
            foreach (var childNode in node.Children.Values)
            {
                AssertActionQualities(childNode);
            }
        }

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
