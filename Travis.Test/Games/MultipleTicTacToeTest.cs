using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Travis.Games.MultipleTicTacToe;
using Travis.Games.MultipleTicTacToe.Heuristics;
using Travis.Logic.Contest;
using Travis.Logic.Learning;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Test.Games
{
    [TestClass]
    public class MultipleTicTacToeTest
    {
        [TestMethod]
        public void MTTTStateTransitionTest()
        {
            var game = new MultipleTicTacToe();
            var state = game.GetInitialState() as MultipleTicTacToeState;
            Assert.AreEqual(TicTacToeEntity.X, state.ControlTicTacToePlayer);
            Assert.AreEqual(0, state.ControlPlayer);
            CustomAssert.AssertState(state);

            var actions = state.GetActionsForActor(0).Values.OfType<MultipleTicTacToeAction>()
                .Where(a => a.BoardNum == 4 && a.PosX == 2 && a.PosY == 1).ToList();
            Assert.AreEqual(1, actions.Count);
            var stateClone = state.Clone() as MultipleTicTacToeState;
            CustomAssert.AssertState(stateClone);

            var aset = state.CreateActionSet(new Dictionary<int, IAction>()
            {
                { 0, actions.Single() },
                { 1, state.GetActionsForActor(1).Values.Single() }
            });
            state.Apply(aset);
            CustomAssert.AssertState(stateClone);
            CustomAssert.AssertState(state, new Dictionary<Tuple<int, int, int>, TicTacToeEntity>()
            {
                { Tuple.Create(4, 2, 1), TicTacToeEntity.X }
            });
            Assert.AreEqual(TicTacToeEntity.X, stateClone.ControlTicTacToePlayer);
            Assert.AreEqual(0, stateClone.ControlPlayer);

            Assert.AreEqual(TicTacToeEntity.O, state.ControlTicTacToePlayer);
            Assert.AreEqual(1, state.ControlPlayer);
        }

        [TestMethod]
        public void MTTTNoWinnerTest()
        {
            var board = new TicTacToeBoard(9);
            board[1, 1] = TicTacToeEntity.X;
            board[0, 0] = TicTacToeEntity.O;
            board[2, 0] = TicTacToeEntity.X;
            board[0, 2] = TicTacToeEntity.O;
            board[0, 1] = TicTacToeEntity.X;
            board[2, 1] = TicTacToeEntity.O;
            board[2, 2] = TicTacToeEntity.X;
            board[1, 2] = TicTacToeEntity.O;
            board[1, 0] = TicTacToeEntity.X;
            Assert.AreEqual(TicTacToeEntity.None, board.Winner);
        }

        [TestMethod]
        public void MTTTBestPolicyProperLearningTest()
        {
            var game = new MultipleTicTacToe();
            var state = game.GetInitialState() as MultipleTicTacToeState;
            var budget = new IterationBasedBudgetProvider(1000);
            var learning = new TreeSearchProcessor();
            var baseRoot = new TreeNode();
            var root = baseRoot;
            var actionSelectors = new Dictionary<int, ActionSelector>()
            {
                { 0, new ActionSelector() { TreePolicy = new UCT(), DefaultPolicy = new BestMoveForBoard() } },
                { 1, new ActionSelector() { TreePolicy = new UCT(), DefaultPolicy = new ChooseBoard() } },
            };
            learning.StateTransition += (n, s, a) =>
                ValidateQualities(s as MultipleTicTacToeState, n);

            learning.Process(root, state, game, budget, actionSelectors);
            ValidateQualities(state, root);

            var fAction = SelectMove(state, 0, 4, 1, 1);
            var sAction = Noop(1);
            root = PerformTransition(state, root, fAction, sAction);

            learning.Process(root, state, game, budget, actionSelectors);
            ValidateQualities(state, root);

            fAction = Noop(0);
            sAction = SelectMove(state, 1, 4, 0, 2);
            root = PerformTransition(state, root, fAction, sAction);

            learning.Process(root, state, game, budget, actionSelectors);
            ValidateQualities(state, root);

            fAction = SelectMove(state, 0, 4, 1, 2);
            sAction = Noop(1);
            root = PerformTransition(state, root, fAction, sAction);
            ValidateQualities(state, root);
        }

        [TestMethod]
        public void MTTTBestPolicyProperGameTest()
        {
            var game = new MultipleTicTacToe();
            MultipleTicTacToeState state;
            MCTSActor firstActor, secondActor = null;
            var moves = new List<string>();
            var learningMoves = new List<string>();

            var matchProcessor = new MatchmakingProcessor();
            matchProcessor.StateTransition += (g, s, a) =>
            {
                moves.Add(s.ToString());
                var str = GetQualityString(s, secondActor.currentRoot);
                if (str != null)
                    moves.Add(str);
            };
            matchProcessor.MatchFinished += (g, s) =>
            {
                moves.Add(s.ToString());
                var payoffs = s.GetPayoffs();
                if (payoffs[1] == 0.0)
                    System.IO.File.WriteAllLines(@"C:\Users\TrolleY\Desktop\log.txt", moves.ToArray());
                //Assert.AreNotEqual(0.0, s.GetPayoffs()[1]);
            };

            for (int i = 0; i < 500; i++)
            {
                System.IO.File.AppendAllText(@"C:\Users\TrolleY\Desktop\iters.txt", $"Started iteration {i}\n");
                state = game.GetInitialState() as MultipleTicTacToeState;
                firstActor = new MCTSActor()
                {
                    PlayTimeBudget = new IterationBasedBudgetProvider(1000)
                };
                secondActor = new MCTSActor()
                {
                    PlayTimeBudget = new IterationBasedBudgetProvider(1000),
                    ActionSelectors = new Dictionary<int, ActionSelector>()
                    {
                        { 0, new ActionSelector() { TreePolicy = new UCT(), DefaultPolicy = new BestMoveForBoard() } },
                        { 1, new ActionSelector() { TreePolicy = new UCT(), DefaultPolicy = new ChooseBoard() } },
                    }
                };
                secondActor.learningProcessor.IterationStarted += (n, s) =>
                {
                    learningMoves.Clear();
                    learningMoves.Add($"Game number : {i}");
                };
                secondActor.learningProcessor.StateTransition += (n, s, a) =>
                {
                    learningMoves.Add(s.ToString());
                    learningMoves.Add(n == null ? "DEFAULT" : "UCT");
                    if (n != null)
                    {
                        var str = GetQualityString(s, n);
                        if (str != null)
                            learningMoves.Add(str);
                    }
                    try
                    {
                        ValidateQualities(s as MultipleTicTacToeState, n);
                    }
                    catch
                    {
                        System.IO.File.WriteAllLines(@"C:\Users\TrolleY\Desktop\learning_log.txt", learningMoves.ToArray());
                        throw;
                    }
                };
                secondActor.learningProcessor.IterationFinished += s =>
                {
                    learningMoves.Add(s.ToString());
                };
                moves.Clear();
                moves.Add($"Game number : {i}");
                matchProcessor.Process(game, new[] { firstActor, secondActor });
            }
        }

        private static string GetQualityString(IState s, TreeNode root)
        {
            var mctsState = s as MultipleTicTacToeState;
            if (!s.IsTerminal && root != null && root.Quality != null && root.Quality.NumVisited != 0)
            {
                var actionsForActor = s.GetActionsForActor(mctsState.ControlPlayer);
                var qualities = root.Quality.ActorActionsQualities[mctsState.ControlPlayer];
                var sb = new StringBuilder();
                foreach (var kv in actionsForActor)
                {
                    sb.AppendLine($"[{kv.Key}, {(qualities.ContainsKey(kv.Key) ? qualities[kv.Key].Quality : 0.0)}, {kv.Value.ToString()}]");
                }
                return sb.ToString();
            }
            return null;
        }

        private TreeNode PerformTransition(MultipleTicTacToeState state, TreeNode root, IAction fAction, IAction sAction)
        {
            var aset = state.CreateActionSet(new Dictionary<int, IAction>()
            {
                { 0, fAction },
                { 1, sAction }
            });
            state.Apply(aset);
            return root.Children[aset.ActionSetId];
        }

        private IAction Noop(int actorId)
        {
            return new MultipleTicTacToeAction() { ActorId = actorId, ActionId = 0, IsNoop = true };
        }

        private IAction SelectMove(MultipleTicTacToeState state, int actorId, int boardNum, int x, int y)
        {
            var actions = state.GetActionsForActor(actorId);
            return actions.Values.OfType<MultipleTicTacToeAction>().Single(a => a.BoardNum == boardNum && a.PosX == x && a.PosY == y);
        }

        private static void ValidateQualities(MultipleTicTacToeState state, TreeNode root)
        {
            if (state.IsTerminal || root == null || root.Quality == null || root.Quality.NumVisited == 0)
                return;
            var actionsForActor = state.GetActionsForActor(1);
            var qualities = root.Quality.ActorActionsQualities[1];
            if (qualities.Count != actionsForActor.Count)
                return;

            if (state.Boards[MultipleTicTacToeState.WinningBoard].PlacedNum == 8)
            {
                for (int x = 0; x < state.Boards[MultipleTicTacToeState.WinningBoard].Size; x++)
                    for (int y = 0; y < state.Boards[MultipleTicTacToeState.WinningBoard].Size; y++)
                        if (state.Boards[MultipleTicTacToeState.WinningBoard][x, y] == TicTacToeEntity.None)
                        {
                            var winningBoard = state.Boards[MultipleTicTacToeState.WinningBoard].Clone();
                            winningBoard[x, y] = state.ControlTicTacToePlayer;
                            if (winningBoard.Winner == TicTacToeEntity.None)
                                return;
                        }
            }

            var minWinningBoardQuality = 1.0;
            foreach (var kv in actionsForActor)
            {
                var mtttAction = actionsForActor[kv.Key] as MultipleTicTacToeAction;
                if (mtttAction.BoardNum == MultipleTicTacToeState.WinningBoard)
                    minWinningBoardQuality = Math.Min(minWinningBoardQuality, (qualities.ContainsKey(kv.Key) ? qualities[kv.Key].Quality : 0.0));
            }
            if (minWinningBoardQuality > 0.5)
            {
                foreach (var kv in root.Quality.ActorActionsQualities[1])
                {
                    var mtttAction = actionsForActor[kv.Key] as MultipleTicTacToeAction;
                    if (!mtttAction.IsNoop)
                    {
                        if (mtttAction.BoardNum != MultipleTicTacToeState.WinningBoard)
                            Assert.IsTrue(kv.Value.Quality == 0.0 || kv.Value.Quality < minWinningBoardQuality, $"Quality: {kv.Value.Quality}, Min wining board quality: {minWinningBoardQuality}");
                    }
                }
            }
        }
    }
}
