using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Travis.Games.GreedyNumbers;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using System;

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
    }
}
