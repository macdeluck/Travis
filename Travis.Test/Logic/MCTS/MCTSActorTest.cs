using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Travis.Games.GreedyNumbers;
using Travis.Logic.Contest;
using Travis.Logic.Learning.Model;
using Travis.Logic.MCTS;
using Travis.Logic.Model;

namespace Travis.Test.Logic.MCTS
{
    [TestClass]
    public class MCTSActorTest
    {
        class TestMCTSActor : MCTSActor
        {
            public int StartClock { get; set; }

            public int PlayClock { get; set; }
            
            public TestMCTSActor(int startTimeIterations, int playTimeIterations)
                : base(new IterationBasedBudgetProvider(startTimeIterations), new IterationBasedBudgetProvider(playTimeIterations))
            {
                StartClock = startTimeIterations;
                PlayClock = playTimeIterations;
            }

            public override void OnMatchBegin(IGame game, IState state, int actorId)
            {
                base.OnMatchBegin(game, state, actorId);
                CustomAssert.AssertTree(currentRoot, StartClock);
                previousNum = null;
            }

            private int? previousNum = null;

            public override void OnStateTransition(IState state, ActionSet actionSet)
            {
                CustomAssert.AssertTree(currentRoot, previousNum.HasValue ? previousNum.Value : StartClock + PlayClock, !previousNum.HasValue);
                
                previousNum = currentRoot.Children.ContainsKey(actionSet.ActionSetId) ? currentRoot.Children[actionSet.ActionSetId].Quality.NumVisited : 0;
                previousNum += PlayClock;
                base.OnStateTransition(state, actionSet);
            }

            public override IAction SelectAction(IState state)
            {
                var action = base.SelectAction(state);
                if (previousNum.HasValue)
                    CustomAssert.AssertTree(currentRoot, previousNum.Value, false);
                else CustomAssert.AssertTree(currentRoot, StartClock + PlayClock, true);
                return action;
            }
        }

        [TestMethod]
        public void MCTSActor_ValidTreeEachMove()
        {
            var first = new TestMCTSActor(3, 15);
            var second = new TestMCTSActor(5, 20);
            var game = new GreedyNumbers(2, new Dictionary<int, int>() { { 1, 5 }, { 2, 3 }, { 7, 1 } });
            var processor = new MatchmakingProcessor();
            var state = processor.Process(game, new[] { first, second });
            Assert.AreEqual(1, state.GetPayoffs()[0]);
        }
    }
}
