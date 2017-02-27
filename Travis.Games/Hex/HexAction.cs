using Travis.Logic.Extensions;
using Travis.Logic.Model;

namespace Travis.Games.Hex
{
    /// <summary>
    /// Represents <see cref="Hex"/> game action.
    /// </summary>
    public class HexAction : IAction
    {
        /// <summary>
        /// Action identifier.
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Actor identifier.
        /// </summary>
        public int ActorId { get; set; }

        /// <summary>
        /// True if action is no operation.
        /// </summary>
        public bool IsNoop { get; set; }

        /// <summary>
        /// The x position of field to capture.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The y position of field to capture.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Returns object string representation.
        /// </summary>
        public override string ToString()
        {
            if (IsNoop) return "Noop";
            return "Place at ({0}, {1})".FormatString(X, Y);
        }
    }
}
