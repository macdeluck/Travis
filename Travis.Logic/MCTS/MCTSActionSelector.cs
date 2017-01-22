using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Learning.Model;

namespace Travis.Logic.MCTS
{
    /// <summary>
    /// Static class containing factory method for MCTS Action Selector.
    /// </summary>
    public static class MCTSActionSelector
    {
        /// <summary>
        /// Creates basic <see cref="ActionSelector"/> for each actor.
        /// </summary>
        /// <param name="actors">Actor ids received from <see cref="IGame.EnumerateActors"/>.</param>
        public static IDictionary<int, ActionSelector> Create(IEnumerable<int> actors)
        {
            return actors.ToDictionary(i => i, i => new ActionSelector()
            {
                DefaultPolicy = new RandomDefaultPolicy(),
                TreePolicy = new UCT()
            });
        }
    }
}
