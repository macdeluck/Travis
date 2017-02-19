using System.Collections.Generic;

namespace Travis.Logic.Model
{
    /// <summary>
    /// Represents game.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Returs number of actors for particular game.
        /// </summary>
        int NumberOfActors { get; }

        /// <summary>
        /// Enumerates actors Ids.
        /// </summary>
        IEnumerable<int> EnumerateActors();

        /// <summary>
        /// Returns initial state for game.
        /// </summary>
        IState GetInitialState();

        /// <summary>
        /// Gets game name.
        /// </summary>
        string Name { get; }
    }
}
