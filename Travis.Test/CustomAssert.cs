using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Travis.Games.GreedyNumbers;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using System;
using Travis.Logic.Learning.Model;
using Travis.Games.MultipleTicTacToe;

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

        /// <summary>
        /// Checks if state is valid.
        /// </summary>
        /// <param name="state">State to assert.</param>
        /// <param name="filledFields">Fields which are expected to be already filled.</param>
        public static void AssertState(MultipleTicTacToeState state)
        {
            AssertState(state, new Dictionary<Tuple<int, int, int>, MTTTPlayer>());
        }

        /// <summary>
        /// Checks if state is valid.
        /// </summary>
        /// <param name="state">State to assert.</param>
        /// <param name="filledFields">Fields which are expected to be already filled.</param>
        public static void AssertState(MultipleTicTacToeState state, Dictionary<Tuple<int, int, int>, MTTTPlayer> filledFields)
        {
            for (int k = 0; k < MultipleTicTacToeState.BoardsNum; k++)
            {
                var board = state.GetBoard(k);
                for (int i = 0; i < MultipleTicTacToeState.BoardSize; i++)
                    for (int j = 0; j < MultipleTicTacToeState.BoardSize; j++)
                    {
                        MTTTPlayer expectedValue;
                        if (!filledFields.TryGetValue(Tuple.Create(k, i, j), out expectedValue))
                            Assert.AreEqual(MTTTPlayer.None, board[i, j]);
                        else Assert.AreEqual(expectedValue, board[i, j]);
                    }
            }
            var actions = state.GetActionsForActor(1 - state.CurrentPlayerId).Select(kv => kv.Value).OfType<MultipleTicTacToeAction>().ToList();
            Assert.AreEqual(1, actions.Count);
            Assert.IsTrue(actions.Single().IsNoop);
            
            bool[][,] actionForBoards = new bool[MultipleTicTacToeState.BoardsNum][,];
            for (int i = 0; i < actionForBoards.Length; i++)
                actionForBoards[i] = new bool[MultipleTicTacToeState.BoardSize, MultipleTicTacToeState.BoardSize];

            int actionsSet = 0;
            foreach (var fd in filledFields)
            {
                if (fd.Value != MTTTPlayer.None)
                {
                    actionForBoards[fd.Key.Item1][fd.Key.Item2, fd.Key.Item3] = true;
                    actionsSet++;
                }
            }

            actions = state.GetActionsForActor(state.CurrentPlayerId).Select(kv => kv.Value).OfType<MultipleTicTacToeAction>().ToList();
            Assert.AreEqual(MultipleTicTacToeState.BoardsNum *
                MultipleTicTacToeState.BoardSize * MultipleTicTacToeState.BoardSize -
                actionsSet,
                actions.Count);

            foreach (var a in actions)
            {
                Assert.IsFalse(a.IsNoop);
                if (actionForBoards[a.BoardNum][a.XPosition, a.YPosition])
                {
                    MTTTPlayer fieldValue;
                    if (filledFields.TryGetValue(Tuple.Create(a.BoardNum, a.XPosition, a.YPosition), out fieldValue) && fieldValue != MTTTPlayer.None)
                        Assert.Fail($"Action for ignored field {a.BoardNum} - ({a.XPosition}, {a.YPosition})");
                    Assert.Fail($"Doubled action for board {a.BoardNum} - ({a.XPosition}, {a.YPosition})");
                }
                else
                {
                    actionsSet++;
                    actionForBoards[a.BoardNum][a.XPosition, a.YPosition] = true;
                }
            }
            Assert.AreEqual(MultipleTicTacToeState.BoardsNum *
                MultipleTicTacToeState.BoardSize * MultipleTicTacToeState.BoardSize, actionsSet);
        }
    }
}
