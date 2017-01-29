namespace Travis.Logic.Learning.Model
{
    /// <summary>
    /// Provides iteration based computational budget.
    /// </summary>
    public class IterationBasedBudgetProvider : IBudgetProvider
    {
        /// <summary>
        /// Maximum number of iterations.
        /// </summary>
        public int MaxIterations { get; private set; }

        /// <summary>
        /// Returns current iteration.
        /// </summary>
        public int Iteration { get; private set; }

        /// <summary>
        /// Creates new instance of class.
        /// <param name="maxIterations">Maximum number of iterations.</param>
        /// </summary>
        public IterationBasedBudgetProvider(int maxIterations)
        {
            MaxIterations = maxIterations;
        }

        /// <summary>
        /// Checks if there is computational budget left to use.
        /// </summary>
        public bool HasBudgetLeft()
        {
            return Iteration < MaxIterations;
        }

        /// <summary>
        /// Indicates next iteration.
        /// </summary>
        public void Next()
        {
            Iteration++;
        }

        /// <summary>
        /// Starts using computational budget.
        /// </summary>
        public void Start()
        {
            Iteration = 0;
        }
    }
}
