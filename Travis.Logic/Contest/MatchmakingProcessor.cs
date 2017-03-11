using System;
using System.Collections.Generic;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Model;

namespace Travis.Logic.Contest
{
    /// <summary>
    /// Executes match between actors.
    /// </summary>
    public class MatchmakingProcessor
    {
        /// <summary>
        /// Runs match of <paramref name="game"/> for specified <paramref name="actors"/>.
        /// </summary>
        /// <param name="game">Game to execute.</param>
        /// <param name="actors">Actors taking part in <paramref name="game"/> exeution.</param>
        /// <returns>Final game state.</returns>
        public IState Process(IGame game, IEnumerable<IActor> actors)
        {
            InitGame(game, actors);
            OnMatchBegin();
            while (!currentState.IsTerminal)
            {
                var actions = GetActionSet();
                OnStateTransition(actions);
                currentState.Apply(actions);
            }
            OnMatchFinished();
            return currentState;
        }

        private ActionSet GetActionSet()
        {
            var actions = new Dictionary<int, IAction>();
            foreach (var actor in actors.Values)
            {
                var action = actor.SelectAction(currentState);
                actions.Add(actor.ActorId, action);
            }
            var actionSet = currentState.CreateActionSet(actions);
            return actionSet;
        }

        private void OnMatchBegin()
        {
            foreach (var kv in actors)
                kv.Value.OnMatchBegin(game, currentState, kv.Key);
            MatchStarted?.Invoke(game, currentState, actors.Values);
        }

        private void OnStateTransition(ActionSet actionSet)
        {
            StateTransition?.Invoke(game, currentState, actionSet);
            foreach (var actor in actors.Values)
                actor.OnStateTransition(currentState, actionSet);
        }
        
        private void OnMatchFinished()
        {
            foreach (var actor in actors.Values)
                actor.OnMatchFinished(currentState);
            MatchFinished?.Invoke(game, currentState);
        }

        #region Events
        /// <summary>
        /// Event invoked when match has been started.
        /// </summary>
        public event Action<IGame, IState, IEnumerable<IActor>> MatchStarted;

        /// <summary>
        /// Event invoked when game state has been changed.
        /// </summary>
        public event Action<IGame, IState, ActionSet> StateTransition;

        /// <summary>
        /// Event invoked when match has been finished.
        /// </summary>
        public event Action<IGame, IState> MatchFinished;
        #endregion

        private IGame game;

        private IDictionary<int, IActor> actors;

        private IState currentState;

        private void InitGame(IGame game, IEnumerable<IActor> actors)
        {
            this.game = game;
            this.actors = new Dictionary<int, IActor>(game.NumberOfActors);
            var actorData = game.EnumerateActors().Zip(actors, (id, actor) => new { ActorId = id, Actor = actor });
            foreach (var actorItem in actorData)
                this.actors.Add(actorItem.ActorId, actorItem.Actor);
            if (this.actors.Count < game.NumberOfActors)
                throw new ArgumentException(Messages.NotEnoughActorsProvided.FormatString(nameof(actors), game.NumberOfActors));
            currentState = game.GetInitialState();
        }
    }
}
