using System.Collections.Generic;
using System.Linq;

namespace Travis.Logic.Learning.Model
{
    /// <summary>
    /// Class containing policies of selecting action during tree learning.
    /// </summary>
    public class ActionSelector
    {
        /// <summary>
        /// Returns tree policy used to select actions during traversing already remembered tree.
        /// </summary>
        public ITreePolicy TreePolicy { get; set; }

        /// <summary>
        /// Returns default policy used to select actions below already remembered tree.
        /// </summary>
        public IDefaultPolicy DefaultPolicy { get; set; }
    }
}
