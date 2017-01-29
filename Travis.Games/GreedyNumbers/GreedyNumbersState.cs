using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Travis.Logic.Extensions;
using Travis.Logic.Model;

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

        /// <summary>
        /// Returns true if state is terminal.
        /// </summary>
        public bool IsTerminal => !PicksAvailable.Any();

        /// <summary>
        /// Gets Id of current actor.
        /// </summary>
        public int CurrentActorId { get; private set; }

        /// <summary>
        /// Applies action set to state and switches to next state.
        /// </summary>
        /// <param name="actionSet">A set of actions taken by actors.</param>
        public void Apply(ActionSet actionSet)
        {
            var action = (GreedyNumbersAction)actionSet.Actions[CurrentActorId];
            PicksAvailable[action.PickValue]--;
            if (PicksAvailable[action.PickValue] == 0)
                PicksAvailable.Remove(action.PickValue);
            Points[CurrentActorId] += action.PickValue;
            CurrentActorId = ParentGame.NextPlayer(CurrentActorId);
        }

        /// <summary>
        /// Clones itself.
        /// </summary>
        public IState Clone()
        {
            return new GreedyNumbersState(PicksAvailable.Clone(), Points.Clone(), CurrentActorId, ParentGame);
        }

        /// <summary>
        /// Creates action set from chosen actions.
        /// </summary>
        /// <param name="actions">Actions chosen by actors keyed with their ids.</param>
        public ActionSet CreateActionSet(IDictionary<int, IAction> actions)
        {
            return new ActionSet()
            {
                Actions = actions,
                ActionSetId = actions[CurrentActorId].ActionId
            };
        }

        /// <summary>
        /// Returns available actions to take by actor.
        /// <param name="actorId">Actor identifier.</param>
        /// </summary>
        /// <returns>Dictionary of actions keyed with theirs identifiers.</returns>
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

        /// <summary>
        /// Gets payoffs for terminal state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Should be thrown when method called on non terminal state.</exception>
        public IDictionary<int, double> GetPayoffs()
        {
            if (!IsTerminal)
                throw new InvalidOperationException();
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

        /// <summary>
        /// Converts game state to its string representation.
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Current actor: {CurrentActorId}");
            sb.AppendFormat("Points: {0}", string.Join(", ", Points.Select(p => $"{p.Key} - {p.Value}")));
            sb.AppendLine();
            sb.AppendFormat("Picks available: {0}", string.Join(", ", PicksAvailable.Select(p => $"{p.Key} - {p.Value}")));
            return sb.ToString();
        }
    }
}
