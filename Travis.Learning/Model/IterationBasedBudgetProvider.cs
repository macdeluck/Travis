namespace Travis.Learning.Model
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

        public bool HasBudgetLeft()
        {
            return Iteration < MaxIterations;
        }

        public void Next()
        {
            Iteration++;
        }

        public void Start()
        {
            Iteration = 0;
        }
    }
}
