using System.Collections.Generic;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Logic.MCTS
{
    /// <summary>
    /// Builder of <see cref="ActionSelector"/>s for game with on-match-begin-assigned actor Id
    /// used by <see cref="MCTSActor"/> for learning purposes.
    /// </summary>
    public interface IActionSelectorBuilder
    {
        /// <summary>
        /// Creates <see cref="ActionSelector"/>s for game.
        /// </summary>
        /// <param name="actorId">Assigned Id for <see cref="MCTSActor"/>.</param>
        /// <param name="game">Game which is going to be played.</param>
        IDictionary<int, ActionSelector> CreateSelectors(int actorId, IGame game);
    }
}
