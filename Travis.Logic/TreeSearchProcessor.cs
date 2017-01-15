using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travis.Logic.BudgetProviders;
using Travis.Logic.Model;

namespace Travis.Logic
{
    /// <summary>
    /// Processes tree with MCTS algorithm.
    /// </summary>
    public class TreeSearchProcessor
    {
        /// <summary>
        /// Runs MCTS algorithm on tree for given problem using default MCTS action selectors.
        /// </summary>
        /// <param name="root">Tree root.</param>
        /// <param name="problem">Problem refering to state.</param>
        /// <param name="iterations">Number of iterations of algorithm to run.</param>
        public void Process(TreeNode root, IProblem problem, int iterations)
        {
            Process(root, problem, iterations, ActionSelector.CreateBasic(problem.EnumerateActors()));
        }

        /// <summary>
        /// Runs MCTS algorithm on tree for given problem.
        /// </summary>
        /// <param name="root">Tree root.</param>
        /// <param name="problem">Problem refering to state.</param>
        /// <param name="iterations">Number of iterations of algorithm to run.</param>
        /// <param name="actionSelectors">Action selectors for actors.</param>
        public void Process(TreeNode root, IProblem problem, int iterations, IDictionary<int, ActionSelector> actionSelectors)
        {
            Process(root, problem.GetInitialState(), problem, iterations, actionSelectors);
        }

        /// <summary>
        /// Runs MCTS algorithm on tree for given problem starting with given state.
        /// </summary>
        /// <param name="root">Tree root.</param>
        /// <param name="rootState">Problem state refering to tree root.</param>
        /// <param name="problem">Problem refering to state.</param>
        /// <param name="iterations">Number of iterations of algorithm to run.</param>
        /// <param name="actionSelectors">Action selectors for actors.</param>
        public void Process(TreeNode root, IState rootState, IProblem problem, int iterations, IDictionary<int, ActionSelector> actionSelectors)
        {
            Process(root, rootState, problem, new IterationBasedBudgetProvider(iterations), actionSelectors);
        }

        /// <summary>
        /// Runs MCTS algorithm on tree for given problem using default MCTS action selectors.
        /// </summary>
        /// <param name="root">Tree root.</param>
        /// <param name="problem">Problem refering to state.</param>
        /// <param name="computationalBudget">Computational budget to run learning.</param>
        public void Process(TreeNode root, IProblem problem, IBudgetProvider computationalBudget)
        {
            Process(root, problem, computationalBudget, ActionSelector.CreateBasic(problem.EnumerateActors()));
        }

        /// <summary>
        /// Runs MCTS algorithm on tree for given problem.
        /// </summary>
        /// <param name="root">Tree root.</param>
        /// <param name="problem">Problem refering to state.</param>
        /// <param name="computationalBudget">Computational budget to run learning.</param>
        /// <param name="actionSelectors">Action selectors for actors.</param>
        public void Process(TreeNode root, IProblem problem, IBudgetProvider computationalBudget, IDictionary<int, ActionSelector> actionSelectors)
        {
            Process(root, problem.GetInitialState(), problem, computationalBudget, actionSelectors);
        }

        /// <summary>
        /// Runs MCTS algorithm on tree for given problem starting with given state.
        /// </summary>
        /// <param name="root">Tree root.</param>
        /// <param name="rootState">Problem state refering to tree root.</param>
        /// <param name="problem">Problem refering to state.</param>
        /// <param name="computationalBudget">Computational budget to run learning.</param>
        /// <param name="actionSelectors">Action selectors for actors.</param>
        public void Process(TreeNode root, IState rootState, IProblem problem, IBudgetProvider computationalBudget, IDictionary<int, ActionSelector> actionSelectors)
        {
            computationalBudget.Start();
            while (computationalBudget.HasBudgetLeft())
            {
                ProcessIteration(root, rootState, problem, actionSelectors);
                computationalBudget.Next();
            }
        }

        /// <summary>
        /// Runs single interation of MCTS algorithm on given tree.
        /// </summary>
        /// <param name="root">Tree root.</param>
        /// <param name="rootState">State refering to tree root.</param>
        /// <param name="problem">Problem refering to state.</param>
        /// <param name="actionSelectors">Action selectors for actors.</param>
        public void ProcessIteration(TreeNode root, IState rootState, IProblem problem, IDictionary<int, ActionSelector> actionSelectors)
        {
            InitIteration(root, rootState, problem, actionSelectors);
            var actionSet = Select();
            Expand(actionSet);
            Simulate();
            Backpropagate();
        }

        #region Common
        private TreeNode currentNode;

        private IState currentState;

        private Stack<Tuple<TreeNode, IActionSet>> decisionPath;

        private IProblem problem;

        private IDictionary<int, ActionSelector> actionSelectors;

        private void InitIteration(TreeNode root, IState rootState, IProblem problem, IDictionary<int, ActionSelector> actionSelectors)
        {
            currentNode = root;
            currentState = rootState.Clone();
            decisionPath = new Stack<Tuple<TreeNode, IActionSet>>();
            this.actionSelectors = actionSelectors;
            this.problem = problem;
        }

        private void PushDecisionPath(IActionSet actionSet)
        {
            decisionPath.Push(Tuple.Create(currentNode, actionSet));
        }

        private void PopDecisionPath(out TreeNode node, out IActionSet actionSet)
        {
            var decisionNode = decisionPath.Pop();
            node = decisionNode.Item1;
            actionSet = decisionNode.Item2;
        }
        #endregion

        #region Selection
        private IActionSet Select()
        {
            while (!currentState.IsTerminal)
            {
                var actionSet = SelectActionsTreePolicy();
                if (currentNode.Children.ContainsKey(actionSet.ActionSetId))
                {
                    PushDecisionPath(actionSet);
                    currentNode = currentNode.Children[actionSet.ActionSetId];
                    currentState.Apply(actionSet);
                }
                else return actionSet;
            }
            return null;
        }

        private IActionSet SelectActionsTreePolicy()
        {
            var actions = new Dictionary<int, IAction>();
            foreach (var actorId in problem.EnumerateActors())
            {
                var action = actionSelectors[actorId].TreePolicy.Invoke(currentNode, currentState, actorId);
                actions.Add(actorId, action);
            }
            var actionSet = currentState.CreateActionSet(actions);
            return actionSet;
        }
        #endregion

        #region Expansion

        private void Expand(IActionSet actionSet)
        {
            if (actionSet != null)
            {
                PushDecisionPath(actionSet);
                currentNode = currentNode.AddNode(actionSet.ActionSetId);
                currentState.Apply(actionSet);
            }
            PushDecisionPath(null);
        }

        #endregion

        #region Simulation

        private void Simulate()
        {
            while (!currentState.IsTerminal)
            {
                var actionSet = SelectActionsDefaultPolicy();
                currentState.Apply(actionSet);
            }
        }

        private IActionSet SelectActionsDefaultPolicy()
        {
            var actions = new Dictionary<int, IAction>();
            foreach (var actorId in problem.EnumerateActors())
            {
                var action = actionSelectors[actorId].DefaultPolicy.Invoke(currentState, actorId);
                actions.Add(actorId, action);
            }
            var actionSet = currentState.CreateActionSet(actions);
            return actionSet;
        }

        #endregion

        #region Backpropagation

        private void Backpropagate()
        {
            TreeNode node;
            IActionSet actionSet;

            var payoffs = currentState.GetPayoffs();
            while (decisionPath.Any())
            {
                PopDecisionPath(out node, out actionSet);
                node.Quality.NumVisited++;
                if (actionSet != null)
                {
                    foreach (var action in actionSet.Actions.Values)
                    {
                        var actionInfo = node.Quality.ActionQuality(action.ActorId, action.ActionId);
                        var S = actionInfo.Quality;
                        var n = actionInfo.NumSelected;
                        actionInfo.Quality = (n * S + payoffs[action.ActorId]) / (n + 1);
                        actionInfo.NumSelected++;
                    }
                }
            }
        }

        #endregion
    }
}
