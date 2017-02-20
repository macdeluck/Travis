using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Travis.Games.FarmingQuandaries;
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
    }
}
