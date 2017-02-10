using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Console.Commandline;
using Travis.Logic.Learning;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Console
{
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
        public void Run(LearnCommandOptions options)
        {
            if (!options.BudgetArgumentList.Any())
                budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(options.BudgetProviderName);
            else budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(options.BudgetProviderName, options.BudgetArgumentList);
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
