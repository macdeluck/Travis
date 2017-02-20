using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Travis.Logic.Model;
using Travis.Logic.Serialization;

namespace Travis.Games.FarmingQuandaries
{
    /// <summary>
    /// A game in which u are farmer.
    /// </summary>
    [UsesSerializer(typeof(FarmingQuandariesGameSerializer))]
    public class FarmingQuandaries : IGame
    {
        /// <summary>
        /// Gets game name.
        /// </summary>
        public string Name => nameof(FarmingQuandaries);

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
            return new FarmingQuandariesState(0, new int[4, 4], new int[2], new bool[2], 0);
        }
    }
}
