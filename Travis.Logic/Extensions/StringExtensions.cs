namespace Travis.Logic.Extensions
{
    /// <summary>
    /// Contains additional methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces the format item with the string representation of a correspondent object in specified array.
        /// </summary>
        /// <param name="formatString">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static string FormatString(this string formatString, params object[] args)
        {
            return string.Format(formatString, args);
        }

        /// <summary>
        /// Checks if given string is null or empty.
        /// </summary>
        /// <param name="str">String to check.</param>
        public static bool HasValue(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
    }
}
