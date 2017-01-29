using System.Collections.Generic;

namespace Travis.Logic.Learning.Model
{
    /// <summary>
    /// Stores node quality data.
    /// </summary>
    public class NodeQualityInfo
    {
        /// <summary>
        /// Number how many times node was visited.
        /// </summary>
        public int NumVisited { get; set; } = 0;

        /// <summary>
        /// Gets information about qualities of actions for particular actor.
        /// </summary>
        public Dictionary<int, ActorQualityInfo> ActorActionsQualities { get; set; } = new Dictionary<int, ActorQualityInfo>();

        /// <summary>
        /// Checks if exists <see cref="ActorQualityInfo"/> for actor.
        /// </summary>
        /// <param name="actorId">Actor identifier.</param>
        public bool ContainsActorQuality(int actorId)
        {
            return ActorActionsQualities.ContainsKey(actorId);
        }

        /// <summary>
        /// Checks if exists action quality info for given actor.
        /// </summary>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="actionId">Action identifier.</param>
        public bool ContainsActionQuality(int actorId, int actionId)
        {
            return ContainsActorQuality(actorId) && ActorActionsQualities[actorId].ContainsActionQuality(actionId);
        }
    }
}
