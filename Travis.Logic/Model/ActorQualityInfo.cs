using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Logic.Model
{
    /// <summary>
    /// Stores actions quality for actor.
    /// </summary>
    public class ActorQualityInfo : Dictionary<int, ActionQualityInfo>
    {
        /// <summary>
        /// Gets action quality info.
        /// </summary>
        /// <param name="actionId">Action identifier took from <see cref="IAction.ActionId"/>.</param>
        public ActionQualityInfo ActionQuality(int actionId)
        {
            if (!ContainsKey(actionId))
                Add(actionId, new ActionQualityInfo());
            return this[actionId];
        }
    }
}
