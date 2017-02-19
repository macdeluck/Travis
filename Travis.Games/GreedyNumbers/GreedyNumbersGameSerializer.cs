using System;
using System.IO;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using Travis.Logic.Serialization;

namespace Travis.Games.GreedyNumbers
{
    /// <summary>
    /// Serializer of <see cref="GreedyNumbers"/> game.
    /// </summary>
    public class GreedyNumbersGameSerializer : IGameSerializer
    {
        /// <summary>
        /// Deserializes action from given stream.
        /// </summary>
        /// <param name="game">Source game.</param>
        /// <param name="state">Current state of game.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="reader">Input stream.</param>
        public IAction DeserializeAction(IGame game, IState state, int actorId, TextReader reader)
        {
            var gstate = (GreedyNumbersState)state;
            var line = reader.ReadLine().Trim();
            if (string.Equals(line, "noop", StringComparison.InvariantCultureIgnoreCase))
                return state.GetActionsForActor(actorId).Values.Single();
            else
            {
                var num = line.Parse<int>();
                return gstate.GetActionsForActor(actorId).Values.Single(a =>
                {
                    var ga = a as GreedyNumbersAction;
                    return ga.PickValue == num;
                });
            }
        }

        /// <summary>
        /// Serializes given action and outputs its to stream.
        /// </summary>
        /// <param name="game">Source game.</param>
        /// <param name="writer">Output stream.</param>
        /// <param name="action">Action to serialize.</param>
        public void SerializeAction(IGame game, IAction action, TextWriter writer)
        {
            var ga = action as GreedyNumbersAction;
            if (ga.IsNoop)
                writer.WriteLine("Noop");
            else writer.WriteLine(ga.PickValue);
        }

        /// <summary>
        /// Serializes given state and outputs its to stream.
        /// </summary>
        /// <param name="game">Source game.</param>
        /// <param name="state">State to serialize.</param>
        /// <param name="writer">Output stream.</param>
        public void SerializeState(IGame game, IState state, TextWriter writer)
        {
            writer.Write(state.ToString());
        }
    }
}
