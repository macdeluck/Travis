using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travis.Logic.DefaultPolicies;
using Travis.Logic.TreePolicies;

namespace Travis.Logic.Model
{
    /// <summary>
    /// Class containing policies of selecting action during tree learning.
    /// </summary>
    public class ActionSelector
    {
        /// <summary>
        /// Returns tree policy used to select actions during traversing already remembered tree.
        /// </summary>
        public ITreePolicy TreePolicy { get; set; } = new UCT();

        /// <summary>
        /// Returns default policy used to select actions below already remembered tree.
        /// </summary>
        public IDefaultPolicy DefaultPolicy { get; set; } = new RandomDefaultPolicy();

        /// <summary>
        /// Creates basic <see cref="ActionSelector"/> for each actor.
        /// </summary>
        /// <param name="actors">Actor ids received from <see cref="IProblem.EnumerateActors"/>.</param>
        public static IDictionary<int, ActionSelector> CreateBasic(IEnumerable<int> actors)
        {
            return actors.ToDictionary(i => i, i => new ActionSelector());
        }
    }
}
