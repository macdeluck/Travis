using System;
using System.IO;
using System.Linq;
using Travis.Logic.Extensions;
using Travis.Logic.Model;
using Travis.Logic.Serialization;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// Serializer of <see cref="MultipleTicTacToe"/> game.
    /// </summary>
    public class MultipleTicTacToeGameSerializer : IGameSerializer
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
            var actionsAvailable = state.GetActionsForActor(actorId);
            var line = reader.ReadLine().Trim();
            if (string.Equals(line, "noop", StringComparison.InvariantCultureIgnoreCase))
                return actionsAvailable.Values.Single();
            var indexes = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (indexes.Length != 3)
                throw new InvalidOperationException("Invalid capture field index");
            var num = indexes[0].Parse<int>();
            var posX = indexes[1].Parse<int>();
            var posY = indexes[2].Parse<int>();
            return actionsAvailable.Values.Single(a =>
            {
                var ma = a as MultipleTicTacToeAction;
                return ma.BoardNum == num && ma.PosX == posX && ma.PosY == posY;
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
