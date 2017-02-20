using System;
using System.IO;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using Travis.Logic.Serialization;

namespace Travis.Games.BattleOfNumbers
{
    /// <summary>
    /// Serializer for state and actions of <see cref="BattleOfNumbers"/> game.
    /// </summary>
    public class BattleOfNumbersGameSerializer : IGameSerializer
    {
        private bool auto = false;

        /// <summary>
        /// Deserializes action from given stream.
        /// </summary>
        /// <param name="game">Source game.</param>
        /// <param name="state">Current state of game.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="reader">Input stream.</param>
        public IAction DeserializeAction(IGame game, IState state, int actorId, TextReader reader)
        {
            var actionsAvailable = state.GetActionsForActor(actorId);

            if (auto)
                return actionsAvailable.Values.RandomElement();

            var line = reader.ReadLine().Trim();

            if (string.Equals(line, "auto", StringComparison.InvariantCultureIgnoreCase))
            {
                auto = true;
                return actionsAvailable.Values.RandomElement();
            }

            var split = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidOperationException("Invalid format");
            var pick = split[0].Trim().Parse<int>();
            var endPos = split[1].Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Parse<int>()).ToArray();
            if (endPos.Length != 2)
                throw new InvalidOperationException("Invalid end pos format");
            return actionsAvailable.Values.Select(a => a as BattleOfNumbersAction)
                .Single(a =>
                a.EndPos[0] == endPos[0] &&
                a.EndPos[1] == endPos[1] &&
                a.Pick == pick);
        }

        /// <summary>
        /// Serializes given action and outputs its to stream.
        /// </summary>
        /// <param name="game">Source game.</param>
        /// <param name="writer">Output stream.</param>
        /// <param name="action">Action to serialize.</param>
        public void SerializeAction(IGame game, IAction action, TextWriter writer)
        {
            writer.Write(action.ToString());
            writer.WriteLine();
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
