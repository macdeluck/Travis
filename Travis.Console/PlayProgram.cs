using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Contest;
using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Console
{
    public class PlayOptions
    {
        [Option('g', "game", Required = true, HelpText = "Name of game to learn")]
        public string GameName { get; set; }

        [Option("p1", Required = true, HelpText = "Name of player 1")]
        public string P1Name { get; set; }

        [Option("b1", HelpText = "Budget provider name for player 1")]
        public string P1BudgetProviderName { get; set; }

        [OptionList("ba1", HelpText = "Arguments for budget provider for player 1")]
        public List<string> p1BudgetArguments { get; set; } = new List<string>();

        public List<KeyValuePair<string, string>> P1BudgetArgumentList => p1BudgetArguments.Select(str => str.ToKeyValuePair()).ToList();

        [OptionList("s1", HelpText = "Action selectors names for player 1")]
        public List<string> P1SelectorNames { get; set; } = new List<string>();

        [Option("p2", Required = true, HelpText = "Name of player 2")]
        public string P2Name { get; set; }

        [Option("b2", HelpText = "Budget provider name for player 2")]
        public string P2BudgetProviderName { get; set; }

        [OptionList("ba2", HelpText = "Arguments for budget provider for player 2")]
        public List<string> p2BudgetArguments { get; set; } = new List<string>();

        public List<KeyValuePair<string, string>> P2BudgetArgumentList => p1BudgetArguments.Select(str => str.ToKeyValuePair()).ToList();

        [OptionList("s2", HelpText = "Action selectors names for player 2")]
        public List<string> P2SelectorNames { get; set; } = new List<string>();

        public List<ActorData> GetActorData()
        {
            var actorData = new List<ActorData>();
            if (P1Name.HasValue())
                actorData.Add(new ActorData()
                {
                    Name = P1Name,
                    BudgetArgumentList = P1BudgetArgumentList,
                    BudgetProviderName = P1BudgetProviderName,
                    SelectorNames = P1SelectorNames
                });
            if (P2Name.HasValue())
                actorData.Add(new ActorData()
                {
                    Name = P2Name,
                    BudgetArgumentList = P2BudgetArgumentList,
                    BudgetProviderName = P2BudgetProviderName,
                    SelectorNames = P2SelectorNames
                });
            return actorData;
        }
    }

    public class ActorData
    {
        public string Name { get; set; }
        
        public string BudgetProviderName { get; set; }
        
        public List<KeyValuePair<string, string>> BudgetArgumentList { get; set; }
        
        public List<string> SelectorNames { get; set; } = new List<string>();
    }

    class PlayProgram
    {
        /// <summary>
        /// Runs play program.
        /// </summary>
        /// <param name="options">Learn program options.</param>
        public void Run(PlayOptions options)
        {
            var processor = new MatchmakingProcessor();
            var game = TravisInit.Current.GetObject<IGame>(options.GameName);
            var actorData = options.GetActorData();
            if (actorData.Count != game.NumberOfActors)
                throw new InvalidOperationException($"Invalid number of actors. Specified {actorData.Count}, expected {game.NumberOfActors}");
            var actors = new List<IActor>(game.NumberOfActors);
            foreach (var adata in actorData)
            {
                var actor = TravisInit.Current.GetObject<IActor>(adata.Name);
                if (adata.BudgetProviderName.HasValue())
                {
                    IBudgetProvider budgetProvider;
                    if (!adata.BudgetArgumentList.Any())
                        budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(adata.BudgetProviderName);
                    else budgetProvider = TravisInit.Current.GetObject<IBudgetProvider>(adata.BudgetProviderName, adata.BudgetArgumentList);
                    actor.GetType().GetProperty("PlayTimeBudget").GetSetMethod().Invoke(actor, new object[] { budgetProvider });

                    IDictionary<int, ActionSelector> actionSelectors;
                    if (adata.SelectorNames.Any())
                    {
                        var actorIds = game.EnumerateActors().ToList();
                        if (adata.SelectorNames.Count != actorIds.Count)
                            throw new Exception();
                        actionSelectors = adata.SelectorNames.Zip(actorIds, (n, i) => new { SelectorName = n, ActorId = i })
                            .ToDictionary(sel => sel.ActorId, sel => TravisInit.Current.GetObject<ActionSelector>(sel.SelectorName));
                    }
                    else actionSelectors = MCTSActionSelector.Create(game.EnumerateActors());
                    actor.GetType().GetProperty("ActionSelectors").GetSetMethod().Invoke(actor, new object[] { budgetProvider });
                }
                actors.Add(actor);
            }
            processor.Process(game, actors);
        }
    }
}
