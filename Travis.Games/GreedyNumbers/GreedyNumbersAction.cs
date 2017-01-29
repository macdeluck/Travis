using Travis.Logic.Model;

namespace Travis.Games.GreedyNumbers
{
    /// <summary>
    /// Represents action for <see cref="GreedyNumbers"/> game.
    /// </summary>
    public class GreedyNumbersAction : IAction
    {
        /// <summary>
        /// Returns true if action is no operation.
        /// </summary>
        public bool IsNoop { get; set; }

        /// <summary>
        /// Gets or sets selected pick value.
        /// </summary>
        public int PickValue { get; set; }

        /// <summary>
        /// Gets or sets action identifier.
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Gets or sets actor identifier.
        /// </summary>
        public int ActorId { get; set; }

        /// <summary>
        /// Converts the action to its string representation.
        /// </summary>
        public override string ToString()
        {
            if (IsNoop)
                return "Noop";
            return PickValue.ToString();
        }
    }
}
