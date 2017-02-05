using System.Collections.Generic;

namespace Travis.Console.Commandline
{
    /// <summary>
    /// Stores available commands to execute.
    /// </summary>
    public struct Commands
    {
        /// <summary>
        /// Command to show program help.
        /// </summary>
        public const string Help = "help";

        /// <summary>
        /// Command to learn tree.
        /// </summary>
        public const string Learn = "learn";
    }

    /// <summary>
    /// Stores commandline options data.
    /// </summary>
    public class CommandlineContext
    {
        /// <summary>
        /// Command to execute.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Output message to show on command execution.
        /// </summary>
        public string OutputMessage { get; set; }

        /// <summary>
        /// Stores parameters for learn command.
        /// </summary>
        public LearnCommandOptions LearnOptions { get; set; }
    }

    /// <summary>
    /// Stores options for learn command.
    /// </summary>
    public class LearnCommandOptions
    {
        /// <summary>
        /// Stores game name.
        /// </summary>
        public string GameName { get; set; } = null;

        /// <summary>
        /// Stores name of budget provider.
        /// </summary>
        public string BudgetProviderName { get; set; } = null;

        /// <summary>
        /// Stores budget provider argument list.
        /// </summary>
        public IList<KeyValuePair<string, string>> BudgetArgumentList { get; set; } = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Stores selector names.
        /// </summary>
        public IList<string> SelectorNames { get; set; } = new List<string>();
    }
}
