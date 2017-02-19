using System;
using System.Linq;
using Travis.Logic.Model;
using Travis.Logic.Serialization;

namespace Travis.Console
{
    public class ConsoleActor : IActor
    {
        public int ActorId { get; private set; }

        private IGameSerializer gameSerializer;

        private IGame game;

        public void OnMatchBegin(IGame game, IState state, int actorId)
        {
            ActorId = actorId;
            this.game = game;
            var gameType = game.GetType();
            var usesSerializer = gameType.GetCustomAttributes(typeof(UsesSerializerAttribute), false);
            if (!usesSerializer.Any())
                throw new InvalidOperationException($"Game {game.Name} has not specified serializer. Use {nameof(UsesSerializerAttribute)} to specify it.");
            var serializerType = (usesSerializer.Single() as UsesSerializerAttribute).SerializerType;
            if (!typeof(IGameSerializer).IsAssignableFrom(serializerType))
                throw new InvalidOperationException($"Game {game.Name} serializer does not implement {nameof(IGameSerializer)} interface");
            var serializerConstructor = serializerType.GetConstructor(new Type[0]);
            if (serializerConstructor == null)
                throw new InvalidOperationException($"Game {game.Name} serializer of type {serializerType.FullName} hasn't default constuctor.");
            gameSerializer = (IGameSerializer)serializerConstructor.Invoke(new object[0]);
            System.Console.WriteLine($"Player identifier: {ActorId}");
        }

        public void OnMatchFinished(IState state)
        {
            gameSerializer.SerializeState(game, state, System.Console.Out);
            System.Console.WriteLine();
            System.Console.WriteLine("Game finished. Payoffs:");
            foreach (var kv in state.GetPayoffs())
                System.Console.WriteLine($"{kv.Key}: {kv.Value}");
        }

        public void OnStateTransition(IState state, ActionSet actionSet)
        {
        }

        public IAction SelectAction(IState state)
        {
            gameSerializer.SerializeState(game, state, System.Console.Out);
            System.Console.WriteLine();
            IAction result = null;
            while (result == null)
            {
                try
                {
                    var actionsAvailable = state.GetActionsForActor(ActorId);
                    if (actionsAvailable.Count == 1)
                    {
                        System.Console.WriteLine("Action chosen automatically (single action available)");
                        System.Console.Write("Chosen action: ");
                        result = actionsAvailable.Values.Single();
                        gameSerializer.SerializeAction(game, result, System.Console.Out);
                    }
                    else
                    {
                        System.Console.Write("Select action: ");
                        result = gameSerializer.DeserializeAction(game, state, ActorId, System.Console.In);
                    }
                }
                catch (Exception exc)
                {
                    System.Console.WriteLine($"Action is incorrect: {exc.Message}");
                }
            }
            return result;
        }
    }
}
