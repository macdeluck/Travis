using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// Represents <see cref="MultipleTicTacToe"/> game action.
    /// </summary>
    public class MultipleTicTacToeAction : IAction
    {
        /// <summary>
        /// Returns action identifier.
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Returns actor's identifier.
        /// </summary>
        public int ActorId { get; set; }

        /// <summary>
        /// True, if action is no operation.
        /// </summary>
        public bool IsNoop { get; set; }

        /// <summary>
        /// Selected board number.
        /// </summary>
        public int BoardNum { get; set; }

        /// <summary>
        /// X positon of captured field.
        /// </summary>
        public int XPosition { get; set; }

        /// <summary>
        /// Y position of captured field.
        /// </summary>
        public int YPosition { get; set; }
    }
}
