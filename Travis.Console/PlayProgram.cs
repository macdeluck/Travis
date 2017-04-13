using CommandLine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Travis.Logic.Contest;
using Travis.Logic.Extensions;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Console
{
    class PlayProgram
    {
        /// <summary>
        /// Runs play program.
        /// </summary>
        /// <param name="options">Learn program options.</param>
        public void Run(Options options)
        {
            var processor = new MatchmakingProcessor();
            var game = BuildGame(options);
            var actors = BuildActors(options);
            var reversedActors = BuildReversedActors(options);

            bool sidesSwitched = false;
            var movesList = new List<string>();
            processor.MatchFinished += (g, s) =>
            {
                var p = s.GetPayoffs();
                if (!sidesSwitched)
                    System.Console.WriteLine($"{p[0].ToString(CultureInfo.InvariantCulture)}, {p[1].ToString(CultureInfo.InvariantCulture)}");
                else System.Console.WriteLine($"{p[1].ToString(CultureInfo.InvariantCulture)}, {p[0].ToString(CultureInfo.InvariantCulture)}");
            };
            if (!options.NumOfGames.HasValue)
                processor.Process(game, actors);
            else
            {
                for (int i = 0; i < options.NumOfGames; i++)
                {
                    sidesSwitched = false;
                    processor.Process(game, actors);
                    sidesSwitched = true;
                    processor.Process(game, reversedActors);
                }
            }
        }

        private static IGame BuildGame(Options options)
        {
            return TravisInit.Current.GetObject<IGame>(options.GameName);
        }

        private IEnumerable<IActor> BuildActors(Options options)
        {
            yield return BuildActor(options.Player1, 
                GetPolicy(options.Player1OwnHeuristic, options.Player1OwnHeuristicProbability),
                GetPolicy(options.Player1EnemyHeuristic, options.Player1EnemyHeuristicProbability),
                BuildBudgetProvider(options));
            yield return BuildActor(options.Player2,
                GetPolicy(options.Player2OwnHeuristic, options.Player2OwnHeuristicProbability),
                GetPolicy(options.Player2EnemyHeuristic, options.Player2EnemyHeuristicProbability),
                BuildBudgetProvider(options));
        }

        private IEnumerable<IActor> BuildReversedActors(Options options)
        {
            yield return BuildActor(options.Player2,
                GetPolicy(options.Player2EnemyHeuristic, options.Player2EnemyHeuristicProbability),
                GetPolicy(options.Player2OwnHeuristic, options.Player2OwnHeuristicProbability),
                BuildBudgetProvider(options));
            yield return BuildActor(options.Player1,
                GetPolicy(options.Player1EnemyHeuristic, options.Player1EnemyHeuristicProbability),
                GetPolicy(options.Player1OwnHeuristic, options.Player1OwnHeuristicProbability),
                BuildBudgetProvider(options));
        }

        private IActor BuildActor(string playerName, IDefaultPolicy ownPolicy, IDefaultPolicy enemyPolicy, IBudgetProvider budgetProvider)
        {
            var actor = TravisInit.Current.GetObject<IActor>(playerName);
            if (actor is MCTSActor)
            {
                var mctsActor = actor as MCTSActor;
                mctsActor.PlayTimeBudget = budgetProvider;
                mctsActor.ActionSelectors = new Dictionary<int, ActionSelector>()
                {
                    { 0, new ActionSelector() { DefaultPolicy = ownPolicy, TreePolicy = new UCT() } },
                    { 1, new ActionSelector() { DefaultPolicy = enemyPolicy, TreePolicy = new UCT() } }
                };
            }
            return actor;
        }

        private static IDefaultPolicy GetPolicy(string name, double probability)
        {
            var policy = new ProbabilityMixedDefaultPolicy();
            policy.ProbabilityThreshold = probability;
            policy.OriginalPolicy = name.HasValue() ? TravisInit.Current.GetObject<IDefaultPolicy>(name) : new RandomDefaultPolicy();
            return policy;
        }

        private IBudgetProvider BuildBudgetProvider(Options options)
        {
            if (options.BudgetProviderArgument <= 0) throw new ArgumentException("Budget provider argument should be at least 1");
            switch (options.BudgetProviderName)
            {
                case "iterations":
                    return new IterationBasedBudgetProvider(options.BudgetProviderArgument);
                case "time":
                    return new TimeBasedBudgetProvider(options.BudgetProviderArgument);
                default:
                    throw new ArgumentException($"Unknown budget provider {options.BudgetProviderName}");
            }
        }
    }
}
