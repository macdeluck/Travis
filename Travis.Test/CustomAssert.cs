using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Travis.Games.GreedyNumbers;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using System;
using Travis.Logic.Learning.Model;

namespace Travis.Test
{
    public class CustomAssert
    {
        /// <summary>
        /// Asserts if two <see cref="GreedyNumbersState"/> instances are equal.
        /// </summary>
        /// <param name="first">First state to compare.</param>
        /// <param name="second">Second state to compare.</param>
        public static void AreEqual(GreedyNumbersState first, GreedyNumbersState second)
        {
            Assert.AreEqual(first.CurrentActorId, second.CurrentActorId);
            Assert.AreEqual(first.IsTerminal, second.IsTerminal);
            Assert.IsTrue(first.PicksAvailable.DictionaryEquals(second.PicksAvailable));
            Assert.IsTrue(first.Points.DictionaryEquals(second.Points));
        }

        /// <summary>
        /// Asserts if two <see cref="GreedyNumbersAction"/> instances are equal.
        /// </summary>
        /// <param name="first">First action to compare.</param>
        /// <param name="second">Second action to compare.</param>
        public static void AreEqual(GreedyNumbersAction first, GreedyNumbersAction second)
        {
            Assert.AreEqual(first.ActionId, second.ActionId);
            Assert.AreEqual(first.ActorId, second.ActorId);
            Assert.AreEqual(first.IsNoop, second.IsNoop);
            Assert.AreEqual(first.PickValue, second.PickValue);
        }

        class GreedyNumbersActionAssertComparer : IEqualityComparer<GreedyNumbersAction>
        {
            public bool Equals(GreedyNumbersAction x, GreedyNumbersAction y)
            {
                AreEqual(x, y);
                return true;
            }

            public int GetHashCode(GreedyNumbersAction obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// Asserts if two <see cref="ActionSet"/> instances of <see cref="GreedyNumbers"/> game are equal.
        /// </summary>
        /// <param name="first">First action set to compare.</param>
        /// <param name="second">Second action set to compare.</param>
        public static void AreEqual(ActionSet first, ActionSet second)
        {
            Assert.AreEqual(first.ActionSetId, second.ActionSetId);
            Assert.AreEqual(first.Actions.Count, second.Actions.Count);
            var firstActions = first.Actions.ToDictionary(kv => kv.Key, kv => kv.Value as GreedyNumbersAction);
            var secondsActions = second.Actions.ToDictionary(kv => kv.Key, kv => kv.Value as GreedyNumbersAction);
            Assert.IsTrue(firstActions.DictionaryEquals(secondsActions, new GreedyNumbersActionAssertComparer()));
        }

        /// <summary>
        /// Checks tree structure beggining from node.
        /// </summary>
        /// <param name="node">Root node to check.</param>
        /// <param name="iterations">Iterations of learning performed beggining at given root.</param>
        /// <param name="isRealGameRoot">True if it is real game root (was initially created).</param>
        public static void AssertTree(TreeNode node, int iterations, bool isRealGameRoot = true)
        {
            if (node.IsTerminal)
            {
                Assert.IsFalse(node.Children.Any());
                Assert.IsFalse(node.Quality.ActorActionsQualities.Any());
                return;
            }

            // Sum of children num visited is equal to parent node num visited decremented by one.
            // The exception is for root node, because root node is initially not visited.
            Assert.AreEqual(iterations - (isRealGameRoot ? 0 : 1), node.Children.Sum(ch => ch.Value.Quality.NumVisited), "Incorrect at root: {0}", isRealGameRoot);

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
                AssertTree(childNode, childNode.Quality.NumVisited, false);
            }
        }
    }
}
