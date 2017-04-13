using System;
using System.Collections.Generic;
using Travis.Logic.Extensions;
using Travis.Logic.Learning;
using Travis.Logic.Learning.Model;
using Travis.Logic.Model;

namespace Travis.Logic.MCTS
{
    /// <summary>
    /// Actor using MCTS algorithm.
    /// </summary>
    public class MCTSActor : IActor
    {
        /// <summary>
        /// Processor used to learn tree.
        /// </summary>
        protected TreeSearchProcessor learningProcessor = new TreeSearchProcessor();

        /// <summary>
        /// Current root node.
        /// </summary>
        protected TreeNode currentRoot;

        /// <summary>
        /// Action selectors used to choose moves.
        /// </summary>
        public IDictionary<int, ActionSelector> ActionSelectors { get; set; }

        /// <summary>
        /// Current game.
        /// </summary>
        protected IGame currentGame;

        /// <summary>
        /// Current game state.
        /// </summary>
        protected IState currentState;

        /// <summary>
        /// Budget provider for initial tree expansion.
        /// </summary>
        public IBudgetProvider StartTimeBudget { get; set; } = new IterationBasedBudgetProvider(0);

        /// <summary>
        /// Budget provider for play time tree expansion.
        /// </summary>
        public IBudgetProvider PlayTimeBudget { get; set; } = new IterationBasedBudgetProvider(1000);

        /// <summary>
        /// Returns identifier assigned on match begin.
        /// </summary>
        public int ActorId { get; private set; }

        /// <summary>
        /// Method called on actor when match begins.
        /// </summary>
        /// <param name="game">Game which is executed.</param>
        /// <param name="state">Begin game state.</param>
        /// <param name="actorId">Identifier assigned to player.</param>
        public virtual void OnMatchBegin(IGame game, IState state, int actorId)
        {
            ActorId = actorId;
            currentGame = game;
            currentState = currentGame.GetInitialState();
            currentRoot = new TreeNode();
            if (ActionSelectors == null)
                ActionSelectors = MCTSActionSelector.Create(game.EnumerateActors());
            ProcessLearning(StartTimeBudget);
        }

        private void ProcessLearning(IBudgetProvider budgetProvider = null)
        {
            if (budgetProvider == null)
                budgetProvider = PlayTimeBudget;
            learningProcessor.Process(currentRoot, currentState, currentGame, budgetProvider, ActionSelectors);
        }

        /// <summary>
        /// Method called when game has been finished.
        /// </summary>
        /// <param name="state">Final game state.</param>
        public virtual void OnMatchFinished(IState state)
        {
        }

        /// <summary>
        /// Method called when all actors already choosed their actions.
        /// </summary>
        /// <param name="state">State which will be <paramref name="actionSet"/> applied to.</param>
        /// <param name="actionSet">Actions selected by actors which will be applied to <paramref name="state"/>.</param>
        public virtual void OnStateTransition(IState state, ActionSet actionSet)
        {
            currentState.Apply(actionSet);
            if (!currentRoot.Children.ContainsKey(actionSet.ActionSetId))
                currentRoot.AddNode(actionSet.ActionSetId, currentState.IsTerminal);
            currentRoot = currentRoot.Children[actionSet.ActionSetId];
        }

        /// <summary>
        /// Method called when actor is asked to choose his action in given state.
        /// </summary>
        /// <param name="state">State in which actor should choose his action.</param>
        public virtual IAction SelectAction(IState state)
        {
            ProcessLearning();
            var availableActions = currentState.GetActionsForActor(ActorId);
            var selectedAction = availableActions.ArgMax(a => GetQualityForAction(a.Key)).RandomElement().Value;
            return selectedAction;
        }

        private double GetQualityForAction(int actionId)
        {
            return currentRoot.Quality.ActorActionsQualities[ActorId][actionId].Quality;
        }
    }
}
