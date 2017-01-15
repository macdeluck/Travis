using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using Travis.Logic.Utils;

namespace Travis.Games.GreedyNumbers
{
    /// <summary>
    /// A state of game <see cref="GreedyNumbers"/>.
    /// </summary>
    public class GreedyNumbersState : IState
    {
        /// <summary>
        /// Creates new instance of <see cref="GreedyNumbers"/> game state.
        /// </summary>
        /// <param name="picksAvailable">Pick available in current state.</param>
        /// <param name="points">Points gathered by actors.</param>
        /// <param name="currentActorId">Current actor identifier.</param>
        /// <param name="parentGame">Parent game instance.</param>
        public GreedyNumbersState(IDictionary<int, int> picksAvailable, IDictionary<int, int> points, int currentActorId, GreedyNumbers parentGame)
        {
            PicksAvailable = picksAvailable;
            Points = points;
            CurrentActorId = currentActorId;
            ParentGame = parentGame;
        }

        /// <summary>
        /// Parent game instance.
        /// </summary>
        public GreedyNumbers ParentGame { get; private set; }

        /// <summary>
        /// Returs picks available in current state.
        /// </summary>
        public IDictionary<int, int> PicksAvailable { get; private set; }

        /// <summary>
        /// Returns points gathered by players.
        /// </summary>
        public IDictionary<int, int> Points { get; private set; }

        public bool IsTerminal => !PicksAvailable.Any();

        /// <summary>
        /// Gets Id of current actor.
        /// </summary>
        public int CurrentActorId { get; private set; }

        public void Apply(IActionSet actionSet)
        {
            var action = (GreedyNumbersAction)actionSet.Actions[CurrentActorId];
            PicksAvailable[action.PickValue]--;
            if (PicksAvailable[action.PickValue] == 0)
                PicksAvailable.Remove(action.PickValue);
            Points[CurrentActorId] += action.PickValue;
            CurrentActorId = ParentGame.NextPlayer(CurrentActorId);
        }

        public IState Clone()
        {
            return new GreedyNumbersState(PicksAvailable.Clone(), Points.Clone(), CurrentActorId, ParentGame);
        }

        public IActionSet CreateActionSet(IDictionary<int, IAction> actions)
        {
            return new BasicActionSet()
            {
                Actions = actions,
                ActionSetId = actions[CurrentActorId].ActionId
            };
        }

        public IDictionary<int, IAction> GetActionsForActor(int actorId)
        {
            if (actorId == CurrentActorId)
            {
                return PicksAvailable.Select((kv, i) => new { Id = i, PickValue = kv.Key }).ToDictionary(
                    k => k.Id,
                    k => new GreedyNumbersAction()
                    {
                        ActionId = k.Id,
                        ActorId = actorId,
                        PickValue = k.PickValue,
                        IsNoop = false
                    } as IAction);
            }
            var noopDict = new Dictionary<int, IAction>();
            noopDict.Add(0, new GreedyNumbersAction()
            {
                ActionId = 0,
                ActorId = actorId,
                IsNoop = true
            });
            return noopDict;
        }

        public IDictionary<int, double> GetPayoffs()
        {
            var topPoints = -1;
            var topPointsCount = 0;
            foreach (var kv in Points)
            {
                if (kv.Value > topPoints)
                {
                    topPoints = kv.Value;
                    topPointsCount = 1;
                }
                else if (kv.Value == topPoints)
                {
                    topPointsCount++;
                }
            }
            return Points.ToDictionary(
                kv => kv.Key, 
                kv => kv.Value != topPoints ? 0 : topPointsCount > 1 ? 0.5 : 1);
        }
    }
}
