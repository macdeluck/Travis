using Travis.Logic.Model;

namespace Travis.Games.FarmingQuandaries
{
    /// <summary>
    /// Enumeration of available actions.
    /// </summary>
    public enum FarmingAction
    {
        /// <summary>
        /// Plow action.
        /// </summary>
        Plow = 0,

        /// <summary>
        /// Sow action.
        /// </summary>
        Sow = 1,

        /// <summary>
        /// Water fields action.
        /// </summary>
        Water = 2,

        /// <summary>
        /// Harvest action.
        /// </summary>
        Harvest = 3,

        /// <summary>
        /// Arson action.
        /// </summary>
        Arson = 4
    }

    /// <summary>
    /// Additional methods for <see cref="FarmingAction"/>.
    /// </summary>
    public static class FarmingActions
    {
        /// <summary>
        /// Creates <see cref="FarmingAction"/> from season number.
        /// </summary>
        /// <param name="season">Index of season.</param>
        public static FarmingAction FromSeason(int season)
        {
            return (FarmingAction)season;
        }
    }

    /// <summary>
    /// Represents <see cref="FarmingQuandaries"/> game action.
    /// </summary>
    public class FarmingQuandariesAction : IAction
    {
        /// <summary>
        /// Gets or sets information if action is no operation.
        /// </summary>
        public bool IsNoop { get; set; }

        /// <summary>
        /// Gets or sets identifier of action.
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Gets or sets identifier of actor.
        /// </summary>
        public int ActorId { get; set; }

        /// <summary>
        /// Checks if action is row action.
        /// </summary>
        public bool IsRowAction { get; set; }

        /// <summary>
        /// Index of row or column.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Action took by player.
        /// </summary>
        public FarmingAction FarmingAction { get; set; }

        /// <summary>
        /// Returns string representation of action.
        /// </summary>
        public override string ToString()
        {
            if (IsNoop)
                return "Noop";
            return string.Format("{0} {1} {2}", FarmingAction, IsRowAction ? "row" : "col", Index + 1);
        }
    }
}
