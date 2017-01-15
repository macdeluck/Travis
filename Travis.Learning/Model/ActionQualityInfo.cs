namespace Travis.Learning.Model
{
    /// <summary>
    /// Stores information about quality of particular action.
    /// </summary>
    public class ActionQualityInfo
    {
        /// <summary>
        /// How many times action has been selected.
        /// </summary>
        public int NumSelected { get; set; } = 0;

        /// <summary>
        /// Action empirical quality.
        /// </summary>
        public double Quality { get; set; } = 0;
    }
}
