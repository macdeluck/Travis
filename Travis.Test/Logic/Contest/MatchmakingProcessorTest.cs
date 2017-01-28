using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Travis.Logic.Model;
using System.Linq;
using Travis.Logic.Contest;
using Travis.Games.GreedyNumbers;

namespace Travis.Test.Logic.Contest
{
    class ActionListActor : IActor
    {
        public int MatchStartCount { get; set; } = 0;

        public int StateTransitionCount { get; set; } = 0;

        public int MatchFinishedCount { get; set; } = 0;

        public GreedyNumbersState CurrentState { get; set; } = null;

        public List<GreedyNumbersAction> ActionsToPerform { get; set; } = null;

        public ActionListActor Opponent { get; set; } = null;

        public bool PerformAssert { get; set; } = true;

        public int ActorId { get; private set; } = -1;

        public void OnMatchBegin(IGame game, IState state, int actorId)
        {
            ActorId = actorId;
            MatchStartCount++;
            CurrentState = game.GetInitialState() as GreedyNumbersState;
            if (PerformAssert)
                CustomAssert.AreEqual(CurrentState, state as GreedyNumbersState);
        }

        public void OnMatchFinished(IState state)
        {
            if (PerformAssert)
                CustomAssert.AreEqual(CurrentState, state as GreedyNumbersState);
            MatchFinishedCount++;
        }

        public void OnStateTransition(IState state, ActionSet actionSet)
        {
            if (PerformAssert)
                CustomAssert.AreEqual(CurrentState, state as GreedyNumbersState);
            var actions = new Dictionary<int, IAction>();
            actions.Add(ActorId, ActionsToPerform[StateTransitionCount]);
            actions.Add(Opponent.ActorId, Opponent.ActionsToPerform[StateTransitionCount]);
            if (PerformAssert)
                CustomAssert.AreEqual(CurrentState.CreateActionSet(actions), actionSet);
            Assert.IsFalse(state.IsTerminal);
            CurrentState.Apply(actionSet);
            StateTransitionCount++;
        }

        public IAction SelectAction(IState state)
        {
            var actionSelected = state.GetActionsForActor(ActorId).Values.First();
            Assert.IsFalse(state.IsTerminal);
            if (PerformAssert)
                CustomAssert.AreEqual(actionSelected as GreedyNumbersAction, ActionsToPerform[StateTransitionCount]);
            return actionSelected;
        }
    }

    [TestClass]
    public class MatchmakingProcessorTest
    {
        [TestMethod]
        public void MatchmakingProcessor_ProcessMatch()
        {
            var processor = new MatchmakingProcessor();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 5 }, { 2, 3 }, { 7, 1 } });
            var actors = new[]
            {
                new ActionListActor() { ActionsToPerform = new List<GreedyNumbersAction>()
                    {
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 2 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 7 },
                    } },
                new ActionListActor() { ActionsToPerform = new List<GreedyNumbersAction>()
                    {
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = false, PickValue = 2 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = false, PickValue = 2 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                    } },
            };
            actors[0].Opponent = actors[1];
            actors[1].Opponent = actors[0];
            processor.Process(game, actors);
            Assert.AreEqual(0, actors[0].ActorId);
            Assert.AreEqual(1, actors[1].ActorId);
            Assert.AreEqual(1, actors[0].MatchStartCount);
            Assert.AreEqual(1, actors[1].MatchStartCount);
            Assert.AreEqual(9, actors[0].StateTransitionCount);
            Assert.AreEqual(9, actors[1].StateTransitionCount);
            Assert.AreEqual(1, actors[0].MatchFinishedCount);
            Assert.AreEqual(1, actors[1].MatchFinishedCount);
            Assert.IsTrue(actors[0].CurrentState.IsTerminal);
            Assert.IsTrue(actors[1].CurrentState.IsTerminal);
            Assert.AreEqual(12, actors[0].CurrentState.Points[0]);
            Assert.AreEqual(12, actors[1].CurrentState.Points[0]);
            Assert.AreEqual(6, actors[0].CurrentState.Points[1]);
            Assert.AreEqual(6, actors[1].CurrentState.Points[1]);
            Assert.AreEqual(1, actors[0].CurrentState.GetPayoffs()[0]);
            Assert.AreEqual(0, actors[0].CurrentState.GetPayoffs()[1]);
            Assert.AreEqual(1, actors[1].CurrentState.GetPayoffs()[0]);
            Assert.AreEqual(0, actors[1].CurrentState.GetPayoffs()[1]);
        }

        [TestMethod]
        public void MatchmakingProcessor_TestEvents()
        {
            var processor = new MatchmakingProcessor();
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 5 }, { 2, 3 }, { 7, 1 } });
            int matchStartCount = 0;
            int stateTransitionCount = 0;
            int matchFinishedCount = 0;
            var actors = new[]
                {
                new ActionListActor() {
                    PerformAssert = false,
                    ActionsToPerform = new List<GreedyNumbersAction>()
                    {
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 2 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 0, IsNoop = false, PickValue = 7 },
                    } },
                new ActionListActor() {
                    PerformAssert = false,
                    ActionsToPerform = new List<GreedyNumbersAction>()
                    {
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = false, PickValue = 1 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = false, PickValue = 2 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = false, PickValue = 2 },
                        new GreedyNumbersAction() { ActionId = 0, ActorId = 1, IsNoop = true, PickValue = 0 },
                    } },
            };
            actors[0].Opponent = actors[1];
            actors[1].Opponent = actors[0];

            var state = game.GetInitialState() as GreedyNumbersState;
            processor.MatchStarted += (g, s, ars) =>
            {
                matchStartCount++;
                CustomAssert.AreEqual(state, s as GreedyNumbersState);
                Assert.AreEqual(2, ars.Count());
                Assert.AreEqual(0, ars.ElementAt(0).ActorId);
                Assert.AreEqual(1, ars.ElementAt(1).ActorId);
            };
            processor.StateTransition += (g, s, acs) =>
            {
                CustomAssert.AreEqual(state, s as GreedyNumbersState);
                var actions = new Dictionary<int, IAction>();
                actions.Add(0, actors[0].ActionsToPerform[stateTransitionCount]);
                actions.Add(1, actors[1].ActionsToPerform[stateTransitionCount]);
                CustomAssert.AreEqual(state.CreateActionSet(actions), acs);
                Assert.IsFalse(s.IsTerminal);
                state.Apply(acs);
                stateTransitionCount++;
            };
            processor.MatchFinished += (g, s) =>
            {
                matchFinishedCount++;
                CustomAssert.AreEqual(state, s as GreedyNumbersState);
                Assert.IsTrue(s.IsTerminal);
            };
            processor.Process(game, actors);
            Assert.AreEqual(1, matchStartCount);
            Assert.AreEqual(9, stateTransitionCount);
            Assert.AreEqual(1, matchFinishedCount);
            Assert.IsTrue(state.IsTerminal);
            Assert.AreEqual(12, state.Points[0]);
            Assert.AreEqual(6, state.Points[1]);
            Assert.AreEqual(1, state.GetPayoffs()[0]);
            Assert.AreEqual(0, state.GetPayoffs()[1]);
        }
    }
}
