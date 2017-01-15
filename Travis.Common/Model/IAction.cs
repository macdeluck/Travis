namespace Travis.Common.Model
{
    /// <summary>
    /// Action of actor.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Gets index of action.
        /// </summary>
        int ActionId { get; }

        /// <summary>
        /// Gets index of actor.
        /// </summary>
        int ActorId { get; }
    }
}
