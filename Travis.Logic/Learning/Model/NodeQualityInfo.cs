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
        /// Gets <see cref="ActorQualityInfo"/> for actor.
        /// </summary>
        /// <param name="actorId">Actor identifier.</param>
        public ActorQualityInfo ActorQuality(int actorId)
        {
            if (!ActorActionsQualities.ContainsKey(actorId))
                ActorActionsQualities.Add(actorId, new ActorQualityInfo());
            return ActorActionsQualities[actorId];
        }

        /// <summary>
        /// Gets action quality info for given actor.
        /// </summary>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="actionId">Action identifier.</param>
        public ActionQualityInfo ActionQuality(int actorId, int actionId)
        {
            return ActorQuality(actorId).ActionQuality(actionId);
        }
    }
}
