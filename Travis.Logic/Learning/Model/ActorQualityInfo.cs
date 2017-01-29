using System.Collections.Generic;

namespace Travis.Logic.Learning.Model
{
    /// <summary>
    /// Stores actions quality for actor.
    /// </summary>
    public class ActorQualityInfo : Dictionary<int, ActionQualityInfo>
    {
        /// <summary>
        /// Checks if exists quality info for action.
        /// </summary>
        /// <param name="actionId">Action identifier took from <see cref="IAction.ActionId"/>.</param>
        public bool ContainsActionQuality(int actionId)
        {
            return ContainsKey(actionId);
        }
    }
}
