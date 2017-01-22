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
        private TreeSearchProcessor learningProcessor;

        private TreeNode currentRoot;

        private IDictionary<int, ActionSelector> actionSelectors;

        private IGame currentGame;

        private IState currentState;

        /// <summary>
        /// Budget provider for initial tree expansion.
        /// </summary>
        public IBudgetProvider StartTimeBudget { get; private set; }

        /// <summary>
        /// Budget provider for play time tree expansion.
        /// </summary>
        public IBudgetProvider PlayTimeBudget { get; private set; }

        /// <summary>
        /// Builder of action selectors on match begin.
        /// </summary>
        public IActionSelectorBuilder ActionSelectorBuilder { get; private set; }

        /// <summary>
        /// Creates new instance of MCTS actor with default action selector builder
        /// and no budget for initial tree expansion.
        /// </summary>
        /// <param name="playTimeIterations">Number of iterations to perform during play time.</param>
        public MCTSActor(int playTimeIterations)
            : this(new IterationBasedBudgetProvider(playTimeIterations), MCTSActionSelector.GetBuilder())
        {
        }

        /// <summary>
        /// Creates new instance of MCTS actor with default action selector builder
        /// and no budget for initial tree expansion.
        /// </summary>
        /// <param name="playTimeBudget">Budget to select action during game.</param>
        public MCTSActor(IBudgetProvider playTimeBudget) : this(playTimeBudget, MCTSActionSelector.GetBuilder())
        {
        }

        /// <summary>
        /// Creates new instance of MCTS actor with no budget for initial tree expansion.
        /// </summary>
        /// <param name="startTimeBudget">Budget to initial tree expansion.</param>
        /// <param name="actionSelectorBuilder">Builder of action selectors on match begin.</param>
        public MCTSActor(IBudgetProvider playTimeBudget, IActionSelectorBuilder actionSelectorBuilder)
            : this(new IterationBasedBudgetProvider(0), playTimeBudget, actionSelectorBuilder)
        {
        }

        /// <summary>
        /// Creates new instance of MCTS actor with default action selector builder.
        /// </summary>
        /// <param name="startTimeBudget">Budget to initial tree expansion.</param>
        /// <param name="playTimeBudget">Budget to select action during game.</param>
        public MCTSActor(IBudgetProvider startTimeBudget, IBudgetProvider playTimeBudget)
            : this(startTimeBudget, playTimeBudget, MCTSActionSelector.GetBuilder())
        {
        }

        /// <summary>
        /// Creates new instance of MCTS actor.
        /// </summary>
        /// <param name="startTimeBudget">Budget to initial tree expansion.</param>
        /// <param name="playTimeBudget">Budget to select action during game.</param>
        /// <param name="actionSelectorBuilder">Builder of action selectors on match begin.</param>
        public MCTSActor(IBudgetProvider startTimeBudget, IBudgetProvider playTimeBudget, IActionSelectorBuilder actionSelectorBuilder)
        {
            if (startTimeBudget == null) throw new ArgumentNullException(nameof(startTimeBudget));
            if (playTimeBudget == null) throw new ArgumentNullException(nameof(playTimeBudget));
            if (actionSelectorBuilder == null) throw new ArgumentNullException(nameof(actionSelectorBuilder));
            StartTimeBudget = startTimeBudget;
            PlayTimeBudget = playTimeBudget;
            ActionSelectorBuilder = actionSelectorBuilder;
            learningProcessor = new TreeSearchProcessor();
        }

        /// <summary>
        /// Returns identifier assigned on match begin.
        /// </summary>
        public int ActorId { get; private set; }

        /// <summary>
        /// Method called on actor when match begins.
        /// </summary>
        /// <param name="actorId">Identifier assigned to player.</param>
        /// <param name="game">Game which is executed.</param>
        public void OnMatchBegin(int actorId, IGame game)
        {
            ActorId = actorId;
            currentGame = game;
            currentState = currentGame.GetInitialState();
            currentRoot = new TreeNode();
            actionSelectors = ActionSelectorBuilder.CreateSelectors(actorId, game);
            ProcessLearning(StartTimeBudget);
        }

        private void ProcessLearning(IBudgetProvider budgetProvider = null)
        {
            if (budgetProvider == null)
                budgetProvider = PlayTimeBudget;
            learningProcessor.Process(currentRoot, currentState, currentGame, budgetProvider, actionSelectors);
        }

        /// <summary>
        /// Method called when game has been finished.
        /// </summary>
        /// <param name="payoffs">Payoffs received from game.</param>
        public void OnMatchFinished(IDictionary<int, double> payoffs)
        {
        }

        /// <summary>
        /// Method called when all actors already choosed their actions.
        /// </summary>
        /// <param name="state">State which will be <paramref name="actionSet"/> applied to.</param>
        /// <param name="actionSet">Actions selected by actors which will be applied to <paramref name="state"/>.</param>
        public void OnStateTransition(IState state, ActionSet actionSet)
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
        public IAction SelectAction(IState state)
        {
            ProcessLearning();
            var availableActions = currentState.GetActionsForActor(ActorId);
            var selectedAction = availableActions.ArgMax(a => GetQualityForAction(a.Key)).RandomElement().Value;
            return selectedAction;
        }

        private double GetQualityForAction(int actionId)
        {
            return currentRoot.Quality.ActionQuality(ActorId, actionId).Quality;
        }
    }
}
