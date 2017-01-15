using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travis.Learning.Model
{
    /// <summary>
    /// Manages computational budget.
    /// </summary>
    public interface IBudgetProvider
    {
        /// <summary>
        /// Starts using computational budget.
        /// </summary>
        void Start();

        /// <summary>
        /// Indicates next iteration.
        /// </summary>
        void Next();

        /// <summary>
        /// Checks if there is computational budget left to use.
        /// </summary>
        /// <returns></returns>
        bool HasBudgetLeft();
    }
}
