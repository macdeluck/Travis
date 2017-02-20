using System;
using System.IO;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using Travis.Logic.Serialization;

namespace Travis.Games.FarmingQuandaries
{
    /// <summary>
    /// Serializer of <see cref="FarmingQuandaries"/> game.
    /// </summary>
    public class FarmingQuandariesGameSerializer : IGameSerializer
    {
        private bool auto = false;
        private bool autonoop = false;

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
            if (autonoop && (state as FarmingQuandariesState).ControlPlayer != actorId)
                return actionsAvailable.Values.Single(a => (a as FarmingQuandariesAction).IsNoop);

            if (auto)
                return actionsAvailable.Values.RandomElement();

            var line = reader.ReadLine().Trim();

            if (string.Equals(line, "autonoop", StringComparison.InvariantCultureIgnoreCase))
            {
                autonoop = true;
                return actionsAvailable.Values.Single(a => (a as FarmingQuandariesAction).IsNoop);
            }
            if (string.Equals(line, "auto", StringComparison.InvariantCultureIgnoreCase))
            {
                auto = true;
                return actionsAvailable.Values.RandomElement();
            }

            if (string.Equals(line, "noop", StringComparison.InvariantCultureIgnoreCase))
                return actionsAvailable.Values.Single(a => (a as FarmingQuandariesAction).IsNoop);
            var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new InvalidOperationException("Invalid action format.");
            var isRowAction = string.Equals(split[0], "row", StringComparison.InvariantCultureIgnoreCase);
            var index = split[1].Parse<int>();
            return actionsAvailable.Values.Single(a =>
            {
                var fa = a as FarmingQuandariesAction;
                return fa.Index == index && fa.IsRowAction == isRowAction;
            });
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
