using CommandLine;
using CommandLine.Text;
using System;

namespace Travis.Console
{
    /// <summary>
    /// Commandline available options.
    /// </summary>
    public class Options
    {
        [Option("game", Required = true, HelpText = "Name of game to learn")]
        public string GameName { get; set; }

        [Option("budget", Required = false, HelpText = "Budget provider name", DefaultValue = "iterations")]
        public string BudgetProviderName { get; set; }

        [Option("budget-argument", Required = false, HelpText = "Budget provider argument. Number of iterations or milliseconds of learning.", DefaultValue = 1000)]
        public int BudgetProviderArgument { get; set; }

        [Option("num", Required = false, HelpText = "Number of games on single side")]
        public int? NumOfGames { get; set; } = null;

        [Option("player1", Required = true, HelpText = "Type of player 1")]
        public string Player1 { get; set; }

        [Option("player1-own-heuristic", Required = false, HelpText = "Heuristic used to simulate own moves in player 1 learning process. Applies for MCTS actor only.")]
        public string Player1OwnHeuristic { get; set; }

        [Option("player1-enemy-heuristic", Required = false, HelpText = "Heuristic used to simulate enemy moves in player 1 learning process. Applies for MCTS actor only.")]
        public string Player1EnemyHeuristic { get; set; }

        [Option("player1-own-heuristic-prob", Required = false, HelpText = "Probability to choose own moves using heuristic in player 1 learning process. Applies for MCTS actor only.", DefaultValue = 1.0)]
        public double Player1OwnHeuristicProbability { get; set; }

        [Option("player1-enemy-heuristic-prob", Required = false, HelpText = "Probability to choose enemy moves using heuristic in player 1 learning process. Applies for MCTS actor only.", DefaultValue = 1.0)]
        public double Player1EnemyHeuristicProbability { get; set; }

        [Option("player2", Required = true, HelpText = "Type of player 2")]
        public string Player2 { get; set; }

        [Option("player2-own-heuristic", Required = false, HelpText = "Heuristic used to simulate own moves in player 2 learning process. Applies for MCTS actor only.")]
        public string Player2OwnHeuristic { get; set; }

        [Option("player2-enemy-heuristic", Required = false, HelpText = "Heuristic used to simulate enemy moves in player 2 learning process. Applies for MCTS actor only.")]
        public string Player2EnemyHeuristic { get; set; }

        [Option("player2-own-heuristic-prob", Required = false, HelpText = "Probability to choose own moves using heuristic in player 2 learning process. Applies for MCTS actor only.", DefaultValue = 1.0)]
        public double Player2OwnHeuristicProbability { get; set; }

        [Option("player2-enemy-heuristic-prob", Required = false, HelpText = "Probability to choose enemy moves using heuristic in player 2 learning process. Applies for MCTS actor only.", DefaultValue = 1.0)]
        public double Player2EnemyHeuristicProbability { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }


    /// <summary>
    /// Class which contains program entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        public static void Main(string[] argv)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(argv, options))
                Environment.Exit(Parser.DefaultExitCodeFail);
            new PlayProgram().Run(options);
        }
    }
}
