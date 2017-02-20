using Travis.Logic.Model;

namespace Travis.Games.BattleOfNumbers
{
    /// <summary>
    /// Represents <see cref="BattleOfNumbers"/> game action.
    /// </summary>
    public class BattleOfNumbersAction : IAction
    {
        /// <summary>
        /// Gets index of actor.
        /// </summary>
        public int ActorId { get; set; }

        /// <summary>
        /// Gets index of action.
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Moved pick value.
        /// </summary>
        public int Pick { get; set; }

        /// <summary>
        /// Start pick position.
        /// </summary>
        public int[] BeginPos { get; set; }

        /// <summary>
        /// End pick position.
        /// </summary>
        public int[] EndPos { get; set; }

        /// <summary>
        /// Is action noop.
        /// </summary>
        public bool IsNoop { get; set; }

        /// <summary>
        /// Returns string representation of action.
        /// </summary>
        public override string ToString()
        {
            if (IsNoop) return "Noop";
            return string.Format("{0}: ({1}) -> ({2})", Pick, string.Join(", ", BeginPos), string.Join(", ", EndPos));
        }
    }
}
