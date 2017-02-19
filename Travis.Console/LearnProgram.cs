using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Learning;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Console
{    
    public class LearnOptions
    {
        [Option('g', "game", Required =true, HelpText = "Name of game to learn")]
        public string GameName { get; set; }

        [Option('b', "budget", HelpText = "Budget provider name")]
        public string BudgetProviderName { get; set; }

        [OptionList('l', "budget-arguments", HelpText = "Arguments for budget provider")]
        public List<string> budgetArguments { get; set; } = new List<string>();

        public List<KeyValuePair<string, string>> BudgetArgumentList => budgetArguments.Select(str => str.ToKeyValuePair()).ToList();

        [OptionList('s', "selectors", HelpText = "Action selectors names")]
        public List<string> SelectorNames { get; set; } = new List<string>();
    }

    /// <summary>
    /// Represents learning program.
    /// </summary>
    public class LearnProgram
    {
        private IBudgetProvider budgetProvider;

        private IGame game;

        private IDictionary<int, ActionSelector> actionSelectors;

        /// <summary>
        /// Runs learn program.
        /// </summary>
        /// <param name="options">Learn program options.</param>
        public void Run(LearnOptions options)
        {
            if (!options.BudgetArgumentList.Any())
                budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(options.BudgetProviderName);
            else budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(options.BudgetProviderName, options.BudgetArgumentList.Select(kv => (KeyValuePair<string, string>)kv).ToList());
            game = TravisInit.Current.GetObject<IGame>(options.GameName);
            if (options.SelectorNames.Any())
            {
                var actorIds = game.EnumerateActors().ToList();
                if (options.SelectorNames.Count != actorIds.Count)
                    throw new Exception();
                actionSelectors = options.SelectorNames.Zip(actorIds, (n, i) => new { SelectorName = n, ActorId = i })
                    .ToDictionary(sel => sel.ActorId, sel => TravisInit.Current.GetObject<ActionSelector>(sel.SelectorName));
            }
            else actionSelectors = MCTSActionSelector.Create(game.EnumerateActors());

            var tree = new TreeNode();
            var processor = new TreeSearchProcessor();
            processor.Process(tree, game, budgetProvider, actionSelectors);
        }
    }
}
