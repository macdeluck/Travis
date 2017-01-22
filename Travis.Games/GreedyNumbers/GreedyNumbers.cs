﻿using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Model;

namespace Travis.Games.GreedyNumbers
{
    /// <summary>
    /// Simple game which goals to take as high picks aggregated value as possible.
    /// </summary>
    public class GreedyNumbers : IGame
    {
        /// <summary>
        /// Creates new instance of game.
        /// </summary>
        /// <param name="numberOfPlayers">Number of players in game.</param>
        /// <param name="pickValues">Picks available to take at start of game.</param>
        public GreedyNumbers(int numberOfPlayers, IDictionary<int, int> pickValues)
        {
            NumberOfActors = numberOfPlayers;
            InitialPickValues = pickValues;
        }

        public int NumberOfActors { get; private set; }

        /// <summary>
        /// Picks available to take at start of game.
        /// </summary>
        public IDictionary<int, int> InitialPickValues { get; private set; }

        public IEnumerable<int> EnumerateActors()
        {
            return Enumerable.Range(0, NumberOfActors);
        }

        public int NextPlayer(int actorId)
        {
            return (actorId + 1) % NumberOfActors;
        }

        public IState GetInitialState()
        {
            return new GreedyNumbersState(InitialPickValues.Clone(), EnumerateActors().ToDictionary(i => i, i => 0), EnumerateActors().First(), this);
        }

        public string Name => nameof(GreedyNumbers);
    }
}
