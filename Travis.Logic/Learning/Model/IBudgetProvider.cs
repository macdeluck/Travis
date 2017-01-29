namespace Travis.Logic.Learning.Model
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
        bool HasBudgetLeft();
    }
}
