using System;
using Travis.Logic.Extensions;
using Travis.Logic.Model;

namespace Travis.Games.MultipleTicTacToe
{
    /// <summary>
    /// Represents <see cref="MultipleTicTacToe"/> game action.
    /// </summary>
    public class MultipleTicTacToeAction : IAction
    {
        /// <summary>
        /// Gets or sets information if action is no operation.
        /// </summary>
        public bool IsNoop { get; set; }

        /// <summary>
        /// Action identifier.
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Number of selected board.
        /// </summary>
        public int BoardNum { get; set; }

        /// <summary>
        /// X position of selected field.
        /// </summary>
        public int PosX { get; set; }

        /// <summary>
        /// Y position of selected field.
        /// </summary>
        public int PosY { get; set; }
        
        /// <summary>
        /// Actor identifier.
        /// </summary>
        public int ActorId { get; set; }
        
        /// <summary>
        /// Returns string representation of itself.
        /// </summary>
        public override string ToString()
        {
            if (IsNoop) return "Noop";
            return "Place at ({0}, {1}) on board {2}".FormatString(PosX, PosY, BoardNum);
        }
    }
}
