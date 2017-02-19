using System.IO;
using Travis.Logic.Model;

namespace Travis.Logic.Serialization
{
    /// <summary>
    /// Interface for game serializer.
    /// </summary>
    public interface IGameSerializer
    {
        /// <summary>
        /// Deserializes action from given stream.
        /// </summary>
        /// <param name="game">Source game.</param>
        /// <param name="state">State of game.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="stream">Input stream.</param>
        IAction DeserializeAction(IGame game, IState state, int actorId, TextReader stream);

        /// <summary>
        /// Serializes given action and outputs its to stream.
        /// </summary>
        /// <param name="game">Source game.</param>
        /// <param name="stream">Output stream.</param>
        /// <param name="action">Action to serialize.</param>
        void SerializeAction(IGame game, IAction action, TextWriter stream);

        /// <summary>
        /// Serializes given state and outputs its to stream.
        /// </summary>
        /// <param name="game">Source game.</param>
        /// <param name="stream">Output stream.</param>
        /// <param name="state">State to serialize.</param>
        void SerializeState(IGame game, IState state, TextWriter stream);
    }
}
