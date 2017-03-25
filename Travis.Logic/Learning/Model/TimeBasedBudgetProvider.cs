using System;
using System.Diagnostics;

namespace Travis.Logic.Learning.Model
{
    /// <summary>
    /// Time based budget provider.
    /// </summary>
    public class TimeBasedBudgetProvider : IBudgetProvider
    {
        private TimeSpan startTimeSpan;

        /// <summary>
        /// Creates new instance of budget provider.
        /// <param name="maxMilliseconds">Maximum time of execution.</param>
        /// </summary>
        public TimeBasedBudgetProvider(int maxMilliseconds)
        {
            Milliseconds = maxMilliseconds;
        }

        /// <summary>
        /// Budget time.
        /// </summary>
        public int Milliseconds { get; private set; }

        /// <summary>
        /// Number of iterations elapsed during learning.
        /// </summary>
        public int IterationsElapsed { get; private set; }

        /// <summary>
        /// Checks if there is computational budget left to use.
        /// </summary>
        public bool HasBudgetLeft() => (GetCurrentTime() - startTimeSpan).TotalMilliseconds < Milliseconds;

        /// <summary>
        /// Indicates next iteration.
        /// </summary>
        public void Next()
        {
            IterationsElapsed++;
        }

        /// <summary>
        /// Starts using computational budget.
        /// </summary>
        public void Start()
        {
            IterationsElapsed = 0;
            startTimeSpan = GetCurrentTime();
        }

        private static TimeSpan GetCurrentTime()
        {
            return Process.GetCurrentProcess().TotalProcessorTime;
        }
    }
}
