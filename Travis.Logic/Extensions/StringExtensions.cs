using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Converts string to key-value pair.
        /// </summary>
        /// <param name="kvStr">String to convert.</param>
        /// <param name="separator">Key and value separator.</param>
        public static KeyValuePair<string, string> ToKeyValuePair(this string kvStr, char separator = '=')
        {
            string key = null;
            string value = null;
            int splitindex = kvStr.IndexOf(separator);
            if (splitindex < 0)
                key = kvStr;
            else
            {
                key = kvStr.Substring(0, splitindex);
                if (splitindex != kvStr.Length)
                    value = kvStr.Substring(splitindex + 1);
            }
            return new KeyValuePair<string, string>(key, value);
        }

        /// <summary>
        /// Parses string to specified type.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="str">String to parse.</param>
        public static T Parse<T>(this string str)
        {
            return (T)Convert.ChangeType(str, typeof(T));
        }
    }
}
