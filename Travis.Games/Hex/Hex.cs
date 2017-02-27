using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Model;
using Travis.Logic.Serialization;

namespace Travis.Games.Hex
{
    /// <summary>
    /// A game invented by John Nash which always leads to win or lose.
    /// </summary>
    [UsesSerializer(typeof(HexGameSerializer))]
    public class Hex : IGame
    {
        /// <summary>
        /// Returns name of the game.
        /// </summary>
        public string Name => nameof(Hex);

        /// <summary>
        /// Returns number of players.
        /// </summary>
        public int NumberOfActors { get { return 2; } }

        /// <summary>
        /// Enumerates players identifiers.
        /// </summary>
        public IEnumerable<int> EnumerateActors() => Enumerable.Range(0, NumberOfActors);

        /// <summary>
        /// Returns new initial state for the game.
        /// </summary>
        public IState GetInitialState()
        {
            return new HexState(0, new HexBoard(9));
        }
    }
}
