using System;
using System.Linq;
using Travis.Console.Commandline;
using Travis.Logic.Learning.Model;

namespace Travis.Console
{
    /// <summary>
    /// Represents learning program.
    /// </summary>
    public class LearnProgram
    {
        private IBudgetProvider budgetProvider;

        /// <summary>
        /// Runs learn program.
        /// </summary>
        /// <param name="options">Learn program options.</param>
        public void Run(LearnCommandOptions options)
        {
            if (!options.BudgetArgumentList.Any())
                budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(options.BudgetProviderName);
            else budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(options.BudgetProviderName, options.BudgetArgumentList);
        }
    }
}
