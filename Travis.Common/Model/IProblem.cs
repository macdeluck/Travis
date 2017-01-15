using System.Collections.Generic;

namespace Travis.Common.Model
{
    /// <summary>
    /// Represents decision problem.
    /// </summary>
    public interface IProblem
    {
        /// <summary>
        /// Returs number of actors for particular problem.
        /// </summary>
        int NumberOfActors { get; }

        /// <summary>
        /// Enumerates actors Ids.
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> EnumerateActors();

        /// <summary>
        /// Returns initial state for problem.
        /// </summary>
        IState GetInitialState();

        /// <summary>
        /// Gets problem name.
        /// </summary>
        string Name { get; }
    }
}
