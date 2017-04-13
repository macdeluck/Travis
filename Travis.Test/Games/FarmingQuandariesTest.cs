using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Games.FarmingQuandaries;
using Travis.Games.FarmingQuandaries.Heuristics;
using Travis.Logic.Extensions;
using Travis.Logic.Learning;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Test.Games
{
    [TestClass]
    public class FarmingQuandariesTest
    {
        [TestMethod]
        public void FarmingQuandaries_StateTransition()
        {
            var board = new int[,]
            {
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            var points = new [] { 0, 0 };
            var hasArson = new[] { false, true };
            var fqState = new FarmingQuandariesState(0, board, points, hasArson, 18);
            Assert.AreEqual("Spring", fqState.CurrentSeason);
            Assert.IsFalse(fqState.IsTerminal);
            var fPlayerActions = fqState.GetActionsForActor(0);
            var sPlayerActions = fqState.GetActionsForActor(1);
            Assert.AreEqual(8, fPlayerActions.Count);
            foreach (var a in fPlayerActions)
            {
                var fa = (a.Value as FarmingQuandariesAction);
                Assert.AreEqual(a.Key, fa.ActionId);
                Assert.AreEqual(0, fa.ActorId);
                Assert.AreEqual(FarmingAction.Sow, fa.FarmingAction);
                Assert.IsFalse(fa.IsNoop);
            }
            Assert.AreEqual(4, fPlayerActions.Values.Select(a => a as FarmingQuandariesAction).Where(a => a.IsRowAction).Select(a => a.Index).Distinct().Count());
            Assert.AreEqual(4, fPlayerActions.Values.Select(a => a as FarmingQuandariesAction).Where(a => !a.IsRowAction).Select(a => a.Index).Distinct().Count());
            Assert.AreEqual(9, sPlayerActions.Count);
            Assert.AreEqual(1, sPlayerActions.Values.Where(a => (a as FarmingQuandariesAction).IsNoop).Count());
            Assert.AreEqual(4, sPlayerActions.Values.Select(a => a as FarmingQuandariesAction).Where(a => !a.IsNoop && a.FarmingAction == FarmingAction.Arson && a.IsRowAction).Select(a => a.Index).Distinct().Count());
            Assert.AreEqual(4, sPlayerActions.Values.Select(a => a as FarmingQuandariesAction).Where(a => !a.IsNoop && a.FarmingAction == FarmingAction.Arson && !a.IsRowAction).Select(a => a.Index).Distinct().Count());
            foreach (var a in sPlayerActions)
            {
                Assert.AreEqual(a.Key, a.Value.ActionId);
                Assert.AreEqual(1, a.Value.ActorId);
            }
            var faction = fPlayerActions.Values.Select(a => a as FarmingQuandariesAction).Single(a => !a.IsRowAction && a.Index == 1);
            var saction = sPlayerActions.Values.Select(a => a as FarmingQuandariesAction).Single(a => !a.IsNoop && a.IsRowAction && a.Index == 2);
            var aSet = fqState.CreateActionSet(new Dictionary<int, IAction>() { { 0, faction }, { 1, saction } });
            Assert.AreEqual(48, aSet.ActionSetId);

            fqState.Apply(aSet);

            Assert.AreEqual("Spring", fqState.CurrentSeason);
            Assert.IsFalse(fqState.IsTerminal);
            Assert.AreEqual(1, fqState.ControlPlayer);
            Assert.AreEqual(0, fqState.Points[0]);
            Assert.AreEqual(0, fqState.Points[1]);
            Assert.IsFalse(fqState.HasArson[0]);
            Assert.IsFalse(fqState.HasArson[1]);
            Assert.AreEqual(19, fqState.GameStep);
            var newboard = new int[,]
            {
                { 0, 0, 0, 0 },
                { 2, 2, 0, 2 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    Assert.AreEqual(newboard[x, y], fqState.Board[x, y]);

            fPlayerActions = fqState.GetActionsForActor(1);
            sPlayerActions = fqState.GetActionsForActor(0);
            Assert.AreEqual(8, fPlayerActions.Count);
            foreach (var a in fPlayerActions)
            {
                var fa = (a.Value as FarmingQuandariesAction);
                Assert.AreEqual(a.Key, fa.ActionId);
                Assert.AreEqual(1, fa.ActorId);
                Assert.AreEqual(FarmingAction.Sow, fa.FarmingAction);
                Assert.IsFalse(fa.IsNoop);
            }
            Assert.AreEqual(4, fPlayerActions.Values.Select(a => a as FarmingQuandariesAction).Where(a => a.IsRowAction).Select(a => a.Index).Distinct().Count());
            Assert.AreEqual(4, fPlayerActions.Values.Select(a => a as FarmingQuandariesAction).Where(a => !a.IsRowAction).Select(a => a.Index).Distinct().Count());
            Assert.AreEqual(1, sPlayerActions.Count);
            Assert.AreEqual(1, sPlayerActions.Values.Where(a => (a as FarmingQuandariesAction).IsNoop).Count());
            foreach (var a in sPlayerActions)
            {
                Assert.AreEqual(a.Key, a.Value.ActionId);
                Assert.AreEqual(0, a.Value.ActorId);
            }
        }

        [TestMethod]
        public void FarmingQuandaries_YearPlayerIndexChanging()
        {
            var game = new FarmingQuandaries();
            var state = game.GetInitialState() as FarmingQuandariesState;
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(0, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(1, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(0, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(1, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(0, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(1, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(0, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(1, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(0, s.YearStartPlayer));
            RandomMovesWithCheck(state, (s, i) => Assert.AreEqual(1, s.YearStartPlayer));
            Assert.IsTrue(state.IsTerminal);
        }

        public class FarmingQuandariesPointsChecker : FarmingQuandariesPoints
        {
            public bool RandomChosen { get; set; }

            protected override IAction GetRandomAction(ICollection<IAction> actions)
            {
                RandomChosen = true;
                return base.GetRandomAction(actions);
            }
        }

        [TestMethod]
        public void FarmingQuandaries_PointsHeuristic()
        {
            CheckPointsGame(true, 3);
            CheckPointsGame(false, 2);
        }

        private static void CheckPointsGame(bool selectRow, int index)
        {
            var game = new FarmingQuandaries();
            var state = game.GetInitialState() as FarmingQuandariesState;
            AdvanceYear(state, 0, selectRow, index);
            AdvanceYear(state, 1, selectRow, index);
            AdvanceYear(state, 0, selectRow, index);
            AdvanceYear(state, 1, selectRow, index);
            AdvanceYear(state, 0, selectRow, index);
            AdvanceYear(state, 1, selectRow, index);
            AdvanceYear(state, 0, selectRow, index);
            AdvanceYear(state, 1, selectRow, index);
            AdvanceYear(state, 0, selectRow, index);
            AdvanceYear(state, 1, selectRow, index);
            Assert.IsTrue(state.IsTerminal);
            Assert.AreEqual(50, state.Points[0]);
            Assert.AreEqual(50, state.Points[1]);
        }

        private static void AdvanceYear(FarmingQuandariesState state, int yearControlPlayer, bool selectRow, int index)
        {
            var pointsHeuristic = new FarmingQuandariesPointsChecker();
            for (int j = 0; j < 6; j++)
            {
                pointsHeuristic.RandomChosen = false;
                pointsHeuristic.Invoke(state, yearControlPlayer);
                Assert.IsTrue(pointsHeuristic.RandomChosen);
                pointsHeuristic.RandomChosen = false;
                pointsHeuristic.Invoke(state, 1 - yearControlPlayer);
                Assert.IsTrue(pointsHeuristic.RandomChosen);
                var fa = SelectActionForActor(state, 0, selectRow, index);
                var sa = SelectActionForActor(state, 1, selectRow, index);
                var aset = state.CreateActionSet(new Dictionary<int, IAction>() { { 0, fa }, { 1, sa } });
                state.Apply(aset);
            }
            pointsHeuristic.RandomChosen = false;
            var action = pointsHeuristic.Invoke(state, yearControlPlayer) as FarmingQuandariesAction;
            Assert.IsFalse(pointsHeuristic.RandomChosen);
            Assert.AreEqual(selectRow, action.IsRowAction);
            Assert.AreEqual(index, action.Index);
            pointsHeuristic.RandomChosen = false;
            pointsHeuristic.Invoke(state, 1 - yearControlPlayer);
            Assert.IsTrue(pointsHeuristic.RandomChosen);
            var fa2 = SelectActionForActor(state, 0, selectRow, index);
            var sa2 = SelectActionForActor(state, 1, selectRow, index);
            var aset2 = state.CreateActionSet(new Dictionary<int, IAction>() { { 0, fa2 }, { 1, sa2 } });
            state.Apply(aset2);
            pointsHeuristic.RandomChosen = false;
            action = pointsHeuristic.Invoke(state, yearControlPlayer) as FarmingQuandariesAction;
            Assert.IsTrue(pointsHeuristic.RandomChosen);
            pointsHeuristic.RandomChosen = false;
            pointsHeuristic.Invoke(state, 1 - yearControlPlayer);
            Assert.IsTrue(pointsHeuristic.RandomChosen);
            fa2 = SelectActionForActor(state, 0, selectRow, index);
            sa2 = SelectActionForActor(state, 1, selectRow, index);
            aset2 = state.CreateActionSet(new Dictionary<int, IAction>() { { 0, fa2 }, { 1, sa2 } });
            state.Apply(aset2);
        }

        private static IAction SelectActionForActor(FarmingQuandariesState state, int actorId, bool selectRow, int index)
        {
            if (state.ControlPlayer == actorId)
                return state.GetActionsForActor(actorId).Values.OfType<FarmingQuandariesAction>().First(fa => fa.IsRowAction == selectRow && fa.Index == index);
            return state.GetActionsForActor(actorId).Values.First();
        }

        private static void RandomMoves(FarmingQuandariesState state, int count = 8)
        {
            RandomMovesWithCheck(state, (s, i) => { }, count);
        }

        private static void RandomMovesWithCheck(FarmingQuandariesState state, Action<FarmingQuandariesState, int> assert, int count = 8)
        {
            for (int j = 0; j < 8; j++)
            {
                assert(state, j);
                var fa = state.GetActionsForActor(0).Values.RandomElement();
                var sa = state.GetActionsForActor(1).Values.RandomElement();
                var aset = state.CreateActionSet(new Dictionary<int, IAction>() { { 0, fa }, { 1, sa } });
                state.Apply(aset);
            }
        }

        [TestMethod]
        public void FarmingQuandaries_ActiveRowsAndCols()
        {
            var pointsPolicy = new ActiveRowOrColumn();
            var board = new int[,]
            {
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            var state = new FarmingQuandariesState(0, board.Transpose(), new[] { 0, 0 }, new[] { false, false }, 2);
            var action = pointsPolicy.Invoke(state, 0) as FarmingQuandariesAction;
            Assert.IsTrue(action.IsRowAction);
            Assert.AreEqual(1, action.Index);


            board = new int[,]
            {
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            state = new FarmingQuandariesState(1, board.Transpose(), new[] { 0, 0 }, new[] { false, false }, 3);
            action = pointsPolicy.Invoke(state, 1) as FarmingQuandariesAction;
            Assert.IsFalse(action.IsRowAction && action.Index == 1);

            board = new int[,]
            {
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            state = new FarmingQuandariesState(1, board.Transpose(), new[] { 10, 0 }, new[] { true, true }, 10);
            Assert.AreEqual(1, state.YearStartPlayer);
            action = pointsPolicy.Invoke(state, 1) as FarmingQuandariesAction;
            Assert.IsTrue(action.IsRowAction);
            Assert.AreEqual(1, action.Index);
            action = pointsPolicy.Invoke(state, 0) as FarmingQuandariesAction;
            Assert.IsTrue(action.IsRowAction);
            Assert.AreEqual(1, action.Index);
            Assert.AreEqual(FarmingAction.Arson, action.FarmingAction);

            board = new int[,]
            {
                { 0, 2, 0, 0 },
                { 1, 2, 1, 1 },
                { 0, 2, 0, 0 },
                { 0, 2, 0, 0 }
            };
            state = new FarmingQuandariesState(0, board.Transpose(), new[] { 10, 0 }, new[] { false, false }, 11);
            action = pointsPolicy.Invoke(state, 0) as FarmingQuandariesAction;
            Assert.IsTrue(action.IsRowAction);
            Assert.AreEqual(1, action.Index);

            board = new int[,]
            {
                { 0, 0, 0, 0 },
                { 3, 0, 3, 3 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
            state = new FarmingQuandariesState(0, board.Transpose(), new[] { 10, 0 }, new[] { false, false }, 15);
            action = pointsPolicy.Invoke(state, 0) as FarmingQuandariesAction;
            Assert.AreEqual(FarmingQuandariesSeason.Fall, state.SeasonIndex);
            Assert.IsTrue(action.IsRowAction);
            Assert.AreEqual(1, action.Index);
        }

        public ActionSet GetDefaultActionSetByParams(FarmingQuandariesState state, FarmingAction action, bool isRowAction, int index)
        {
            var selectedActions = new Dictionary<int, IAction>();
            selectedActions.Add(state.ControlPlayer, state.GetActionsForActor(state.ControlPlayer)
                .Values
                .OfType<FarmingQuandariesAction>()
                .First(a => !a.IsNoop && a.FarmingAction == action && a.IsRowAction == isRowAction && a.Index == index));
            selectedActions.Add(1 - state.ControlPlayer, state.GetActionsForActor(1 - state.ControlPlayer)
                .Values
                .OfType<FarmingQuandariesAction>()
                .First(a => a.IsNoop));
            return state.CreateActionSet(selectedActions);
        }
    }
}
