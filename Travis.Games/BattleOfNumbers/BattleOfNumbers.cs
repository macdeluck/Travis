using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Travis.Logic.Model;
using Travis.Logic.Serialization;

namespace Travis.Games.BattleOfNumbers
{
    /// <summary>
    /// Simple checkers-like game.
    /// </summary>
    [UsesSerializer(typeof(BattleOfNumbersGameSerializer))]
    public class BattleOfNumbers : IGame
    {
        /// <summary>
        /// Gets game name.
        /// </summary>
        public string Name => nameof(BattleOfNumbers);

        /// <summary>
        /// Returs number of actors for particular game.
        /// </summary>
        public int NumberOfActors => 2;

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
            return new BattleOfNumbersState(0, CreateStartPicks(), new[] { 0, 0 });
        }

        private IDictionary<int, int[]>[] CreateStartPicks()
        {
            var result = new Dictionary<int, int[]>[]
            {
                new Dictionary<int, int[]>(),
                new Dictionary<int, int[]>(),
            };
            result[0].Add(-1, new[] { 0, 0 });
            result[0].Add(-3, new[] { 0, 1 });
            result[0].Add(-5, new[] { 0, 2 });
            result[0].Add(-4, new[] { 0, 3 });
            result[0].Add(-2, new[] { 0, 4 });
            
            result[1].Add( 2, new[] { 4, 4 });
            result[1].Add( 4, new[] { 4, 3 });
            result[1].Add( 5, new[] { 4, 2 });
            result[1].Add( 3, new[] { 4, 1 });
            result[1].Add( 1, new[] { 4, 0 });
            return result;
        }
    }
}
