namespace Travis.Logic.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces the format item with the string representation of a correspondent object in specified array.
        /// </summary>
        /// <param name="formatString">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FormatException"/>
        public static string FormatString(this string formatString, params object[] args)
        {
            return string.Format(formatString, args);
        }
    }
}
